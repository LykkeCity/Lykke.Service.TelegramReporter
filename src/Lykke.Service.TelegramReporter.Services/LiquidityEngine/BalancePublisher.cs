using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client;
using Lykke.Service.TelegramReporter.Core;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Balance;

namespace Lykke.Service.TelegramReporter.Services.LiquidityEngine
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
            ILogFactory logFactory)
            : base(telegramSender, publisherSettings, logFactory)
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

            var balanceWarnings = await _balanceWarningRepository.GetBalancesWarnings();

            var balancesWallets = balanceWarnings.Select(x => x.ClientId).Distinct()
                .ToDictionary(x => x, x => _balancesClient.GetClientBalances(x));

            var tasks = balancesWallets.Values.Cast<Task>().ToList();
            await Task.WhenAll(tasks);

            var balances = new Dictionary<string, ClientBalanceResponseModel>();
            foreach (var balanceWallet in balancesWallets)
            {
                foreach (var balance in balanceWallet.Value.Result)
                {
                    balances[GetBalanceDictionaryKey(balanceWallet.Key, balance.AssetId)] = balance;
                }
            }

            foreach (var balanceWarning in balanceWarnings)
            {
                var key = GetBalanceDictionaryKey(balanceWarning.ClientId, balanceWarning.AssetId);

                var isBalanceFound = balances.TryGetValue(key, out var balance);

                if (isBalanceFound && balance.Balance < balanceWarning.MinBalance ||
                    !isBalanceFound && balanceWarning.MinBalance > 0)
                {
                    balanceIssues.Add(new BalanceIssueDto
                    {
                        ClientId = balanceWarning.ClientId,
                        AssetId = balanceWarning.AssetId,
                        Name = balanceWarning.Name,
                        AssetName = balanceWarning.AssetName,
                        Balance = balance?.Balance ?? 0,
                        MinBalance = balanceWarning.MinBalance
                    });
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
