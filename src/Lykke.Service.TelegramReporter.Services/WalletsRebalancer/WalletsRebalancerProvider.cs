using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.MarketMakerWalletsRebalancer.Contract;
using Lykke.Service.TelegramReporter.Core.Services.WalletsRebalancer;

namespace Lykke.Service.TelegramReporter.Services.WalletsRebalancer
{
    public class WalletsRebalancerProvider : IWalletsRebalancerProvider
    {
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly ILog _log;

        public WalletsRebalancerProvider(IAssetsServiceWithCache assetsServiceWithCache, ILogFactory logFactory)
        {
            _assetsServiceWithCache = assetsServiceWithCache;
            _log = logFactory.CreateLog(this);
        }

        public async Task<string> GetMessageAsync(RebalanceOperation message)
        {
            var messageText = await GetMessage(message);
            return ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(messageText);
        }

        private async Task<string> GetMessage(RebalanceOperation message)
        {
            var state = new StringBuilder();

            state.AppendLine($"======= {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} =======\r\n");
            state.AppendLine("Wallets Rebalancer event:\r\n");

            state.AppendLine($"Time: {message.Date}");
            state.AppendLine($"Wallet Name: {message.WalletName}");
            state.AppendLine($"Status: {message.Status}");
            state.AppendLine("Description:");
            state.AppendLine($"{await GetDescriptionAsync(message.Assets)}");

            return state.ToString();
        }

        private async Task<string> GetDescriptionAsync(IReadOnlyList<AssetOperation> assetRebalanceOperations)
        {
            var assets = await _assetsServiceWithCache.GetAllAssetsAsync(true);

            var items = new List<string>();

            foreach (var assetRebalanceOperation in assetRebalanceOperations.OrderBy(o => o.AssetId))
            {
                var asset = assets.SingleOrDefault(o => o.Id == assetRebalanceOperation.AssetId);

                var numberFormat = "0.".PadRight((asset?.Accuracy ?? 2) + 2, '0');

                var format = $"+{numberFormat};-{numberFormat};0";

                var amount = assetRebalanceOperation.Amount;

                if (assetRebalanceOperation.Type == ExchangeOperationType.CashOut)
                    amount *= -1;

                items.Add($"{assetRebalanceOperation.AssetId}: {amount.ToString(format)}");
            }

            return string.Join(Environment.NewLine, items);
        }
    }
}
