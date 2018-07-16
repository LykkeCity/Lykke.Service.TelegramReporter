using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.Assets.Client;
using Lykke.Service.SpreadEngine.Client.Extensions;
using Lykke.Service.SpreadEngine.Client.Models;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services.SpreadEngine;

namespace Lykke.Service.TelegramReporter.Services.SpreadEngine
{
    public class SpreadEngineStateProvider : ISpreadEngineStateProvider
    {
        private readonly ISpreadEngineInstanceManager _spreadEngineInstanceManager;
        private readonly IAssetsService _assetsService;

        public SpreadEngineStateProvider(ISpreadEngineInstanceManager spreadEngineInstanceManager,
            IAssetsService assetsService)
        {
            _spreadEngineInstanceManager = spreadEngineInstanceManager;
            _assetsService = assetsService;
        }

        public async Task<string> GetStateMessageAsync()
        {
            var tasks = new List<Task<string>>();
            foreach (var instanceId in _spreadEngineInstanceManager.Instances.Select(x => x.Index))
            {
                tasks.Add(GetStateMessageAsync(instanceId));
            }

            await Task.WhenAll(tasks);

            var sb = new StringBuilder();
            foreach (var task in tasks)
            {
                sb.AppendLine(task.Result);
                sb.AppendLine();
            }
            return ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(sb.ToString());
        }

        public async Task<string> GetStateMessageAsync(int instanceId)
        {
            //var assetPairs = await _assetsService.AssetPairGetAllAsync();

            const string baseAssetId = "BTC";
            var assetPairs = new Dictionary<string, Task<InventoryModel>>
            {
                {"BTCUSD", null},
                {"BTCCHF", null},
                {"BTCJPY", null},
                {"BTCEUR", null},
                {"BTCGBP", null},
                {"BTCAUD", null}
            };

            var balancesTask = _spreadEngineInstanceManager[instanceId]
                .Balances.GetAsync();

            var taskTraders = _spreadEngineInstanceManager[instanceId]
                .Traders.GetAsync(Array.Empty<string>());

            foreach (var assetPairId in assetPairs.Keys.ToList())
            {
                assetPairs[assetPairId] = _spreadEngineInstanceManager[instanceId]
                    .Traders.GetInventoryAsync(assetPairId);
            }

            var tasks = assetPairs.Values.Cast<Task>().ToList();
            tasks.Add(balancesTask);
            tasks.Add(taskTraders);

            await Task.WhenAll(tasks);

            var traders = taskTraders.Result;

            //var assetPairsInTraders = traders.Select(x => x.AssetPairId);
            //var assetPairsForBalances = assetPairs
            //    .Where(x => assetPairsInTraders.Contains(x.Id))
            //    .SelectMany(x => new[] {x.BaseAssetId, x.QuotingAssetId})
            //    .Distinct()
            //    .ToHashSet();

            var balances = balancesTask.Result;

            var assetInventories = new List<AssetInventoryModel>();

            var volume = 0M;

            foreach (var assetPairId in assetPairs.Keys)
            {
                var assetId = assetPairId.Replace(baseAssetId, "");

                var balance = balances.FirstOrDefault(o => o.AssetId == assetId)?.Amount ?? 0m;

                var inventory = assetPairs[assetPairId].Result;

                volume += inventory?.Absolute ?? 0;

                assetInventories.Add(new AssetInventoryModel
                {
                    AssetId = assetId,
                    Balance = balance,
                    BaseVolume = inventory?.Absolute ?? 0,
                    Volume = inventory?.AbsoluteQuote ?? 0
                });
            }

            var baseAssetBalance = balances.FirstOrDefault(o => o.AssetId == baseAssetId)?.Amount ?? 0m;

            assetInventories.Add(new AssetInventoryModel
            {
                AssetId = baseAssetId,
                Balance = baseAssetBalance,
                BaseVolume = 0,
                Volume = volume
            });

            assetInventories = assetInventories.OrderBy(o => o.AssetId).ToList();

            var message = GetStateMessage(traders, balances, assetPairs, assetInventories);
            return ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(message);
        }

        private string GetStateMessage(IReadOnlyList<TraderModel> traders, IReadOnlyList<BalanceModel> balances, IDictionary<string, Task<InventoryModel>> assetPairs, IList<AssetInventoryModel> assetInventories)
        {
            var state = new StringBuilder();

            state.Append($"======= {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} =======\r\n\r\n");
            state.Append($"Spread Engine State:\r\n\r\n");

            foreach (var trader in traders)
            {
                var isFound = assetPairs.TryGetValue(trader.AssetPairId, out var traderInventoryTask);
                var traderInventory = new InventoryModel();
                if (isFound)
                {
                    traderInventory = traderInventoryTask?.Result;
                }

                state.Append(
                    $"{trader.AssetPairId} inv: {traderInventory?.Absolute:0.000}; " +
                    $"({trader.SellVolumeCoefficient:0.000}/{trader.BuyVolumeCoefficient:0.000}); " +
                    //$"daily: <daily sell volume>/<daily buy volume>; " +
                    $"PL: {trader.PnL:0.000}\r\n"
                );
            }

            state.Append("\r\n");
            state.Append("Balances:\r\n\r\n");

            foreach (var inventoryModel in assetInventories)
            {
                state.Append(
                    $"{inventoryModel.AssetId} Balance: {inventoryModel.Balance:0.000} " +
                    $"Inventory: {inventoryModel.Volume:0.000}\r\n"
                );
            }

            return state.ToString();
        }
    }

    internal class AssetInventoryModel
    {
        public string AssetId { get; set; }

        public decimal Volume { get; set; }

        public decimal BaseVolume { get; set; }

        public decimal Balance { get; set; }
    }
}
