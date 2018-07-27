using System.Collections.Generic;
using System.Linq;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.NettingEngine.Client.Models.Balances;
using Lykke.Service.NettingEngine.Client.Models.Instruments;
using Lykke.Service.NettingEngine.Client.Models.InstrumentSettings;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Data
{
    public static class Common
    {
        public static IReadOnlyList<AssetViewModel> GetAssetViewModels(IEnumerable<InstrumentSettingsModel> instrumentSettings, IReadOnlyList<BalanceModel> balances)
        {
            return instrumentSettings
                .Select(o => GetAssetViewModel(o, balances))
                .OrderBy(o => o.Id)
                .ToList();
        }

        public static AssetViewModel GetAssetViewModel(InstrumentSettingsModel instrumentSettings, IReadOnlyList<BalanceModel> balances)
        {
            return new AssetViewModel
            {
                Id = instrumentSettings.InstrumentId,
                Balance = balances.FirstOrDefault(b => b.AssetId == instrumentSettings.InstrumentId)?.Amount ?? 0m,
                Settings = GetInstrumentSettingsViewModel(instrumentSettings, true)
            };
        }

        public static IReadOnlyList<AssetPairViewModel> GetAssetPairViewModels(
            IEnumerable<InstrumentModel> instruments,
            IReadOnlyCollection<AssetPair> assetPairs,
            IReadOnlyList<InstrumentSettingsModel> instrumentSettings,
            IReadOnlyList<BalanceModel> balances)
        {
            var list = new List<AssetPairViewModel>();

            foreach (InstrumentModel instrument in instruments)
            {
                var settings = instrumentSettings.FirstOrDefault(o => o.InstrumentId == instrument.AssetPairId);

                if (settings == null)
                    continue;

                list.Add(GetAssetPairViewModel(instrument, assetPairs, settings, balances));
            }

            return list
                .OrderBy(o => o.Active ? 0 : 1)
                .ThenBy(o => o.Id)
                .ToList();
        }

        public static AssetPairViewModel GetAssetPairViewModel(
            InstrumentModel instrument,
            IReadOnlyCollection<AssetPair> assetPairs,
            InstrumentSettingsModel instrumentSettings,
            IReadOnlyList<BalanceModel> balances)
        {
            AssetPair assetPair = assetPairs.Single(o => o.Id == instrument.AssetPairId);

            return new AssetPairViewModel
            {
                Id = instrument.AssetPairId,
                Active = instrument.Behavior == InstrumentBehavior.Active,
                PnL = instrument.PnL,
                SellVolumeCoefficient = instrument.SellVolumeCoefficient,
                BuyVolumeCoefficient = instrument.BuyVolumeCoefficient,
                QuoteAssetBalance = balances
                                        .FirstOrDefault(o => o.AssetId == assetPair.QuotingAssetId)?.Amount ?? 0m,
                Settings = GetInstrumentSettingsViewModel(instrumentSettings, true),
                LykkeQuote = new QuoteViewModel(instrument.LykkeExchangeQuote.Ask,
                    instrument.LykkeExchangeQuote.Bid),
                ExternalQuote = new QuoteViewModel(instrument.ExternalExchangeQuote.Ask,
                    instrument.ExternalExchangeQuote.Bid)
            };
        }

        public static InstrumentSettingsViewModel GetInstrumentSettingsViewModel(InstrumentSettingsModel model, bool allowEmpty)
        {
            return new InstrumentSettingsViewModel
            {
                TargetSpread = model.Spread,
                BestLevelVolume = model.Volume,
                Depth = model.Depth,
                SideSumVolume = model.SideSumVolume,
                ExternalExchange = model.QuoteSource,
                ThresholdFractionToCorrectOrders = model.Threshold,
                AlphaVolumeFactor = model.Alpha,
                SpreadScalingFactor = model.SpreadScaling,
                AllowArbitrage = GetBooleanItems(model.Arbitrage, allowEmpty),
                TraderTypes = new List<ItemViewModel>
                {
                    new ItemViewModel(TraderType.TargetSpread.ToString(), model.TraderType == TraderType.TargetSpread),
                    new ItemViewModel(TraderType.QuoteSpread.ToString(), model.TraderType == TraderType.QuoteSpread)
                }
            };
        }

        public static IReadOnlyList<ItemViewModel> GetBooleanItems(bool? value, bool allowEmpty)
        {
            var items = new List<ItemViewModel>();

            if (allowEmpty)
                items.Add(new ItemViewModel());

            items.Add(new ItemViewModel("Yes", value == true));
            items.Add(new ItemViewModel("No", value == false));

            return items;
        }
    }
}
