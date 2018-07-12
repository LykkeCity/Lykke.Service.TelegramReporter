using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.NettingEngine.Client.Models.Instruments;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;
using Lykke.Service.NettingEngine.Client.Extensions;
using Lykke.Service.RateCalculator.Client;
using Lykke.Service.TelegramReporter.Services.NettingEngine.Data;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine
{
    public class NettingEngineStateProvider : INettingEngineStateProvider
    {
        private readonly INettingEngineInstanceManager _nettingEngineInstanceManager;
        private readonly IAssetsService _assetsService;
        private readonly IRateCalculatorClient _rateCalculatorClient;

        public NettingEngineStateProvider(INettingEngineInstanceManager nettingEngineInstanceManager,
            IAssetsService assetsService,
            IRateCalculatorClient rateCalculatorClient)
        {
            _nettingEngineInstanceManager = nettingEngineInstanceManager;
            _assetsService = assetsService;
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

        public async Task<string> GetStateMessageAsync(int instanceId)
        {
            var assetPairs = (await _assetsService.AssetPairGetAllAsync())
                .ToArray();

            var assetPairList = assetPairs
                //.Where(o => model.Assets.Contains(o.BaseAssetId))
                .Select(o => o.Id)
                .ToArray();

            var instrumentsTask = _nettingEngineInstanceManager[instanceId]
                .Instruments.GetAsync(Array.Empty<string>());
            var assetSettingsTask = _nettingEngineInstanceManager[instanceId]
                .InstrumentSettings.GetAssetSettingsAsync(Array.Empty<string>());
            var assetPairSettingsTask = _nettingEngineInstanceManager[instanceId]
                .InstrumentSettings.GetAssetPairSettingsAsync(Array.Empty<string>());
            var balancesTask = _nettingEngineInstanceManager[instanceId]
                .Balances.GetAsync();
            var ratesTask = _rateCalculatorClient.GetMarketProfileAsync();

            await Task.WhenAll(instrumentsTask, assetSettingsTask, assetPairSettingsTask, balancesTask, ratesTask);

            var instruments = instrumentsTask.Result;
            var assetSettings = assetSettingsTask.Result;
            var assetPairSettings = assetPairSettingsTask.Result;
            var balances = balancesTask.Result;
            var marketProfile = ratesTask.Result;

            var assetPairsData =
                Data.Common.GetAssetPairViewModels(instruments, assetPairs, assetPairSettings, balances);
            var assets = Data.Common.GetAssetViewModels(assetSettings, balances);
            var assetPairVm = Data.Common.GetAssetPairViewModels(instruments, assetPairs, assetPairSettings, balances);
            var inventoryTasks = assetPairVm.Select(assetPair => _nettingEngineInstanceManager[instanceId]
                .InstrumentInventory.GetAsync(assetPair.Id)).ToArray();

            await Task.WhenAll(inventoryTasks);

            var inventories = inventoryTasks.Select(task => task.Result).ToList();

            var assetsData =
                Data.Common.GetAssetSummary(assets, assetPairs, inventories, marketProfile, instruments);

            var message = GetStateMessage(assetPairsData, assetsData);
            return ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(message);
        }

        private string GetStateMessage(IReadOnlyList<AssetPairViewModel> assetPairsData, IReadOnlyList<SummaryAssetViewModel> assetsData)
        {
            var state = new StringBuilder();

            state.Append($"======= {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} =======\r\n\r\n");
            state.Append($"Netting Engine State:\r\n\r\n");

            foreach (var assetPair in assetPairsData)
            {
                state.Append(
                    $"{assetPair.Id} " +
                    $"({assetPair.SellVolumeCoefficient:0.000}/{assetPair.BuyVolumeCoefficient:0.000}); " +
                    //$"daily: <daily sell volume>/<daily buy volume>; " +
                    $"PL: {assetPair.PnL:0.000}\r\n"
                );
            }

            state.Append("\r\n");
            state.Append("Balances:\r\n");

            foreach (var assetViewModel in assetsData)
            {
                state.Append(
                    $"{assetViewModel.Asset} Balance: {assetViewModel.Balance:0.000} " +
                    $"Inventory: {assetViewModel.AbsoluteInventory:0.000} TotalSell: {assetViewModel.TotalSell:0.000} " +
                    $"TotalBuy: {assetViewModel.TotalBuy:0.000} PL: {assetViewModel.TotalPnL??0:0.000}\r\n"
                );
            }

            return state.ToString();
        }
    }
}
