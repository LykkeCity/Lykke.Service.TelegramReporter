using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Balance;

namespace Lykke.Service.TelegramReporter.Services.Balance
{
    public class ExternalBalancePublisher : ChatPublisher
    {
        private readonly IExternalBalanceWarningRepository _externalBalanceWarningRepository;
        private readonly INettingEngineInstanceManager _nettingEngineInstanceManager;
        private readonly IExternalBalanceWarningProvider _externalBalanceWarningProvider;

        public ExternalBalancePublisher(ITelegramSender telegramSender,
            IExternalBalanceWarningRepository externalBalanceWarningRepository,
            INettingEngineInstanceManager nettingEngineInstanceManager,
            IExternalBalanceWarningProvider externalBalanceWarningProvider,
            IChatPublisherSettings publisherSettings,
            ILogFactory logFactory)
            : base(telegramSender, publisherSettings, logFactory)
        {
            _externalBalanceWarningRepository = externalBalanceWarningRepository;
            _nettingEngineInstanceManager = nettingEngineInstanceManager;
            _externalBalanceWarningProvider = externalBalanceWarningProvider;
        }

        public override async void Publish()
        {
            try
            {
                var balancesWithIssues = await FindBalancesWithIssues();

                if (balancesWithIssues.Count > 0)
                {
                    await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId,
                        await _externalBalanceWarningProvider.GetWarningMessageAsync(balancesWithIssues));
                }
            }
            catch (Exception ex)
            {
                await Log.WriteErrorAsync(nameof(BalancePublisher), nameof(Publish), "", ex);
            }
        }

        private async Task<IList<ExternalBalanceIssueDto>> FindBalancesWithIssues()
        {
            var balanceIssues = new List<ExternalBalanceIssueDto>();

            var balanceWarnings = (await _externalBalanceWarningRepository.GetExternalBalancesWarnings())
                .ToDictionary(x => GetBalanceDictionaryKey(x.Exchange, x.AssetId), x => x);

            var neInstance = _nettingEngineInstanceManager.Instances.First();
            if (neInstance != null)
            {
                var task = neInstance.Balances.GetExternalAsync();
                await Task.WhenAll(task);

                var balances = task.Result
                    .ToDictionary(x => GetBalanceDictionaryKey(x.Exchange, x.AssetId), x => x);

                foreach (var balanceWarning in balanceWarnings)
                {
                    var isBalanceFound = balances.TryGetValue(balanceWarning.Key, out var balance);

                    if (isBalanceFound && balance.Amount < balanceWarning.Value.MinBalance ||
                        !isBalanceFound && balanceWarning.Value.MinBalance > 0)
                    {
                        balanceIssues.Add(new ExternalBalanceIssueDto
                        {
                            Exchange = balanceWarning.Value.Exchange.ToUpperInvariant(),
                            AssetId = balanceWarning.Value.AssetId.ToUpperInvariant(),
                            Name = balanceWarning.Value.Name,
                            Balance = balance?.Amount ?? 0,
                            MinBalance = balanceWarning.Value.MinBalance
                        });
                    }
                }
            }

            return balanceIssues;
        }

        private string GetBalanceDictionaryKey(string exchange, string assetId)
        {
            return $"{exchange.ToUpperInvariant()}_{assetId.ToUpperInvariant()}";
        }
    }    
}
