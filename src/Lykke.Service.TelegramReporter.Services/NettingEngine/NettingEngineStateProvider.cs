using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;
using Lykke.Service.NettingEngine.Client.Models.Balances;
using Lykke.Service.NettingEngine.Client.Models.HedgeLimitOrders;
using Lykke.Service.NettingEngine.Client.Models.Inventories;
using Lykke.Service.RateCalculator.Client;
using Lykke.Service.RateCalculator.Client.AutorestClient.Models;
using Lykke.Service.TelegramReporter.Services.NettingEngine.Data;
using AssetPair = Lykke.Service.Assets.Client.Models.AssetPair;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine
{
    public class NettingEngineStateProvider : INettingEngineStateProvider
    {
        private readonly INettingEngineInstanceManager _nettingEngineInstanceManager;
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly IRateCalculatorClient _rateCalculatorClient;

        public NettingEngineStateProvider(INettingEngineInstanceManager nettingEngineInstanceManager,
            IAssetsServiceWithCache assetsServiceWithCache,
            IRateCalculatorClient rateCalculatorClient)
        {
            _nettingEngineInstanceManager = nettingEngineInstanceManager;
            _assetsServiceWithCache = assetsServiceWithCache;
            _rateCalculatorClient = rateCalculatorClient;
        }

        public async Task<string> GetStateMessageAsync()
        {
            var tasks = new List<Task<string>>();
            foreach (var instanceId in _nettingEngineInstanceManager.Instances.Select(x => x.Index))
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

        public async Task<string> GetStateMessageAsync(int serviceInstanceIndex)
        {
            var inventory = await GetInventory(serviceInstanceIndex);

            var message = GetStateMessage(inventory);
            return ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(message);
        }

        private async Task<InventoryReportViewModel> GetInventory(int serviceInstanceIndex)
        {
            NettingEngineInstance nettingEngineInstance = _nettingEngineInstanceManager[serviceInstanceIndex];

            Task<IReadOnlyList<InventoryModel>> inventoryTask = nettingEngineInstance.Inventory.GetAllAsync();
            Task<IReadOnlyList<HedgeLimitOrderModel>> hedgeLimitOrdersTask =
                nettingEngineInstance.HedgeLimitOrders.GetAllAsync();
            Task<IReadOnlyList<BalanceModel>> lykkeBalancesTask = nettingEngineInstance.Balances.GetLykkeAsync();
            Task<IReadOnlyList<BalanceModel>> externalBalancesTask =
                nettingEngineInstance.Balances.GetExternalAsync();
            Task<IReadOnlyCollection<Asset>> assetsTask = _assetsServiceWithCache.GetAllAssetsAsync(false);
            Task<IReadOnlyCollection<AssetPair>> assetPairsTask = _assetsServiceWithCache.GetAllAssetPairsAsync();
            Task<MarketProfile> marketProfileTask = _rateCalculatorClient.GetMarketProfileAsync();

            await Task.WhenAll(
                inventoryTask,
                hedgeLimitOrdersTask,
                lykkeBalancesTask,
                externalBalancesTask,
                assetsTask,
                assetPairsTask,
                marketProfileTask);

            IReadOnlyList<InventoryModel> inventory = inventoryTask.Result;
            IReadOnlyList<HedgeLimitOrderModel> hedgeLimitOrders = hedgeLimitOrdersTask.Result;
            IReadOnlyList<BalanceModel> balances = lykkeBalancesTask.Result
                .Union(externalBalancesTask.Result)
                .ToList();
            IReadOnlyCollection<Asset> assets = assetsTask.Result;
            IReadOnlyCollection<AssetPair> assetPairs = assetPairsTask.Result;
            MarketProfile marketProfile = marketProfileTask.Result;

            IReadOnlyList<string> exchanges = inventory.Select(o => o.Exchange).Distinct().ToArray();

            if (exchanges.Contains("lykke"))
                exchanges = new[] { "lykke" }.Union(exchanges.OrderBy(o => o)).Distinct().ToList();

            IReadOnlyList<string> inventoryAssets = inventory.Select(o => o.AssetId).Distinct().ToArray();

            var rows = new List<RowViewModel>();

            var summary = new Dictionary<string, List<ExchangeSummaryViewModel>>();

            foreach (string assetId in inventoryAssets)
            {
                Asset asset = assets.FirstOrDefault(o => o.Id == assetId);
                HedgeLimitOrderModel hedgeLimitOrder = hedgeLimitOrders.FirstOrDefault(o => o.Asset == assetId);

                var exchangeSummary = new List<ExchangeSummaryViewModel>();

                foreach (string exchange in exchanges)
                {
                    BalanceModel balance = balances
                        .FirstOrDefault(o => o.AssetId == assetId && o.Exchange == exchange);

                    InventoryModel assetInventory = inventory
                        .FirstOrDefault(o => o.AssetId == assetId && o.Exchange == exchange);

                    var assetExchangeSummary = new ExchangeSummaryViewModel
                    {
                        Balance = balance?.Amount ?? 0,
                        BalanceUsd = (balance?.Amount ?? 0) * UsdRate(assetId, assetPairs, marketProfile),
                        BalanceRatio = 0,
                        Inventory = assetInventory?.Volume ?? 0,
                        InventoryUsd = (assetInventory?.Volume ?? 0) * UsdRate(assetId, assetPairs, marketProfile),
                        Sell = assetInventory?.SellVolume ?? 0,
                        Buy = assetInventory?.BuyVolume ?? 0
                    };

                    if (!summary.ContainsKey(exchange))
                        summary[exchange] = new List<ExchangeSummaryViewModel>();

                    summary[exchange].Add(assetExchangeSummary);
                    exchangeSummary.Add(assetExchangeSummary);
                }

                rows.Add(new RowViewModel
                {
                    Asset = new ItemViewModel(assetId, asset?.DisplayId ?? assetId),
                    HedgeMode = hedgeLimitOrder?.Mode.ToString(),
                    Exchanges = exchangeSummary,
                    Summary = new ExchangeSummaryViewModel
                    {
                        Balance = exchangeSummary.Sum(o => o.Balance),
                        BalanceUsd = exchangeSummary.Sum(o => o.BalanceUsd),
                        Inventory = exchangeSummary.Sum(o => o.Inventory),
                        InventoryUsd = exchangeSummary.Sum(o => o.InventoryUsd),
                        Sell = exchangeSummary.Sum(o => o.Sell),
                        Buy = exchangeSummary.Sum(o => o.Buy)
                    }
                });
            }

            var total = new List<ExchangeTotalViewModel>();

            foreach (KeyValuePair<string, List<ExchangeSummaryViewModel>> pair in summary)
            {
                var exchangeTotal = new ExchangeTotalViewModel
                {
                    BalanceUsd = pair.Value.Sum(o => o.BalanceUsd),
                    InventoryUsd = pair.Value.Sum(o => o.InventoryUsd)
                };

                total.Add(exchangeTotal);

                if (exchangeTotal.BalanceUsd == 0)
                    continue;

                foreach (ExchangeSummaryViewModel exchangeSummary in pair.Value)
                    exchangeSummary.BalanceRatio = exchangeSummary.BalanceUsd / Math.Abs(exchangeTotal.BalanceUsd);
            }

            decimal totalBalanceUsd = Math.Abs(total.Sum(o => o.BalanceUsd));

            if (totalBalanceUsd > 0)
            {
                foreach (RowViewModel row in rows)
                    row.Summary.BalanceRatio = row.Summary.BalanceUsd / totalBalanceUsd;
            }

            var vm = new InventoryReportViewModel
            {
                Exchanges = exchanges,
                Rows = rows,
                Total = new TotalRowViewModel
                {
                    Exchanges = total
                }
            };

            return vm;
        }

        private string GetStateMessage(InventoryReportViewModel inventoryReportViewModel)
        {
            var state = new StringBuilder();

            state.AppendLine($"======= {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} =======\r\n");
            state.AppendLine("Netting Engine State:\r\n");

            foreach (var row in inventoryReportViewModel.Rows)
            {
                state.Append(
                    $"{row.Asset.Title} Inventory: {row.Summary.Inventory:0.000}"
                );

                var externalInventory = row.Summary.Inventory - row.Exchanges.First().Inventory;
                if (externalInventory != 0)
                {
                    state.Append(
                        $" External: {externalInventory:0.000}"
                    );
                }

                state.AppendLine();
            }

            state.AppendLine();
            state.AppendLine($"Trading PnL: {inventoryReportViewModel.Total.Exchanges.Sum(o => o.InventoryUsd):0.000}\r\n");

            state.AppendLine("Lykke Turnover:\r\n");

            foreach (var row in inventoryReportViewModel.Rows)
            {
                state.AppendLine(
                    $"{row.Asset.Title} Total: {(row.Exchanges.First().Sell + row.Exchanges.First().Buy):0.000} " +
                    $"Buy: {row.Exchanges.First().Buy:0.000} " +
                    $"Sell: {row.Exchanges.First().Sell:0.000} "
                );
            }

            return state.ToString();
        }

        private decimal UsdRate(string assetId, IReadOnlyCollection<AssetPair> assetPairts, MarketProfile marketProfile)
        {
            const string quoteAssetId = "USD";

            if (assetId == quoteAssetId)
                return 1;

            AssetPair directAssetPair = assetPairts
                .FirstOrDefault(o => o.BaseAssetId == assetId && o.QuotingAssetId == quoteAssetId);

            if (directAssetPair != null)
            {
                FeedData feedData = marketProfile.Profile.FirstOrDefault(o => o.Asset == directAssetPair.Id);

                if (feedData != null)
                    return (decimal)(feedData.Ask + feedData.Bid) / 2m;
            }

            AssetPair invertedAssetPair = assetPairts
                .FirstOrDefault(o => o.BaseAssetId == quoteAssetId && o.QuotingAssetId == assetId);

            if (invertedAssetPair != null)
            {
                FeedData feedData = marketProfile.Profile.FirstOrDefault(o => o.Asset == invertedAssetPair.Id);

                if (feedData != null)
                    return 1 / ((decimal)(feedData.Ask + feedData.Bid) / 2m);
            }

            return 0;
        }
    }
}
