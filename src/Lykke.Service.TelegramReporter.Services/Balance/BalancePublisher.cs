﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.Balances.Client;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Balance;

namespace Lykke.Service.TelegramReporter.Services.Balance
{
    public class BalancePublisher : ChatPublisher
    {
        private readonly IBalanceWarningRepository _balanceWarningRepository;
        private readonly IBalancesClient _balancesClient;
        private readonly IBalanceWarningProvider _balanceWarningProvider;

        public BalancePublisher(ITelegramSender telegramSender,
            IBalanceWarningRepository balanceWarningRepository,
            IBalancesClient balancesClient,
            IBalanceWarningProvider balanceWarningProvider,
            IChatPublisherSettings publisherSettings,
            ILog log)
            : base(telegramSender, publisherSettings, log)
        {
            _balanceWarningRepository = balanceWarningRepository;
            _balancesClient = balancesClient;
            _balanceWarningProvider = balanceWarningProvider;
        }

        public override async void Publish()
        {
            try
            {
                var balancesWithIssues = await FindBalancesWithIssues();

                if (balancesWithIssues.Count > 0)
                {
                    await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId,
                        await _balanceWarningProvider.GetWarningMessageAsync(balancesWithIssues));
                }
            }
            catch (Exception ex)
            {
                await Log.WriteErrorAsync(nameof(BalancePublisher), nameof(Publish), "", ex);
            }
        }

        private async Task<IList<BalanceIssueDto>> FindBalancesWithIssues()
        {
            var balanceIssues = new List<BalanceIssueDto>();

            var balanceWarnings = (await _balanceWarningRepository.GetBalancesWarnings())
                .ToDictionary(x => GetBalanceDictionaryKey(x.ClientId, x.AssetId), x => x);

            var balancesWallets = balanceWarnings.Values.Select(x => x.ClientId).Distinct()
                .ToDictionary(x => x, x => _balancesClient.GetClientBalances(x));

            var tasks = balancesWallets.Values.Cast<Task>().ToList();
            await Task.WhenAll(tasks);

            foreach (var balanceWallet in balancesWallets)
            {
                var balances = balanceWallet.Value.Result;
                foreach (var balance in balances)
                {
                    var isWarningFound = balanceWarnings.TryGetValue(
                        GetBalanceDictionaryKey(balanceWallet.Key, balance.AssetId), out var balanceWarning);

                    if (isWarningFound)
                    {
                        if (balance.Balance < balanceWarning.MinBalance)
                        {
                            balanceIssues.Add(new BalanceIssueDto
                            {
                                ClientId = balanceWallet.Key,
                                AssetId = balance.AssetId,
                                Balance = balance.Balance,
                                MinBalance = balanceWarning.MinBalance
                            });
                        }
                    }
                }
            }

            return balanceIssues;
        }

        private string GetBalanceDictionaryKey(string clientId, string assetId)
        {
            return $"{clientId}_{assetId}";
        }
    }    
}