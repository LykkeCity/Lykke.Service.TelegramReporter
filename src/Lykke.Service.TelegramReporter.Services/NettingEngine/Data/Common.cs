using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.NettingEngine.Client.Models.Balances;
using Lykke.Service.NettingEngine.Client.Models.Instruments;
using Lykke.Service.NettingEngine.Client.Models.OrderBooks;
using Lykke.Service.RateCalculator.Client.AutorestClient.Models;
using AssetPair = Lykke.Service.Assets.Client.Models.AssetPair;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Data
{
    public class Common
    {
        public static IReadOnlyList<AssetViewModel> GetAssetViewModels(
            IEnumerable<Lykke.Service.NettingEngine.Client.Models.InstrumentSettings.InstrumentSettingsModel>
                instrumentSettings, IReadOnlyList<BalanceModel> balances)
        {
            return instrumentSettings
                .Select(o => GetAssetViewModel(o, balances))
                .OrderBy(o => o.Id)
                .ToList();
        }

        public static AssetViewModel GetAssetViewModel(
            Lykke.Service.NettingEngine.Client.Models.InstrumentSettings.InstrumentSettingsModel
                instrumentSettings, IReadOnlyList<BalanceModel> balances)
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
            IReadOnlyList<Lykke.Service.NettingEngine.Client.Models.InstrumentSettings.InstrumentSettingsModel>
                instrumentSettings, IReadOnlyList<BalanceModel> balances)
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
            Lykke.Service.NettingEngine.Client.Models.InstrumentSettings.InstrumentSettingsModel instrumentSettings,
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

        public static InstrumentSettingsViewModel GetInstrumentSettingsViewModel(
            Lykke.Service.NettingEngine.Client.Models.InstrumentSettings.InstrumentSettingsModel model, bool allowEmpty)
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
                Hedging = GetBooleanItems(model.Hedging, allowEmpty),
                Exchange = model.Exchange,
                Tier1 = model.Tier1,
                Tier2 = model.Tier2,
                Tier3 = model.Tier3
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

        public static IReadOnlyList<SummaryAssetViewModel> GetAssetSummary(
            IReadOnlyList<AssetViewModel> assets,
            IReadOnlyCollection<AssetPair> assetPairs,
            IReadOnlyList<Lykke.Service.NettingEngine.Client.Models.Inventories.InstrumentInventoryModel> inventories,
            MarketProfile marketProfile,
            IReadOnlyList<InstrumentModel> instruments
            )
        {
            var feedData = marketProfile.Profile.ToDictionary(feed => feed.Asset, feed => feed);

            var extInventories =
                from inventory in inventories
                join assetPair in assetPairs
                    on inventory.AssetPairId equals assetPair.Id
                join instrument in instruments
                    on inventory.AssetPairId equals instrument.AssetPairId
                select new
                {
                    inventory.AssetPairId,
                    inventory.BaseAssetVolume,
                    inventory.BuyVolume,
                    inventory.SellVolume,
                    assetPair.BaseAssetId,
                    assetPair.QuotingAssetId,
                    instrument.PnL
                };

            return assets.Select(a =>
            {
                var inv = extInventories
                    .Where(i => string.Equals(i.BaseAssetId, a.Id, StringComparison.InvariantCultureIgnoreCase))
                    .ToArray();

                var convertedTotalPnL =
                    inv.Sum(i => i.PnL * GetConvertRatio(i.QuotingAssetId, "USD", assetPairs, feedData));

                return new SummaryAssetViewModel
                {
                    Asset = a.Id,
                    Balance = a.Balance,
                    AbsoluteInventory = inv.Sum(i => i.BaseAssetVolume),
                    TotalBuy = inv.Sum(i => i.BuyVolume),
                    TotalSell = inv.Sum(i => i.SellVolume),
                    TotalPnL = convertedTotalPnL
                };
            }).ToList();
        }

        private static decimal? GetConvertRatio(string fromAssetId, string toAssetId,
            IEnumerable<AssetPair> assetPairs,
            IDictionary<string, FeedData> feedData)
        {
            var assetPair = assetPairs.FirstOrDefault(ap =>
                (ap.BaseAssetId == toAssetId && ap.QuotingAssetId == fromAssetId)
                || (ap.BaseAssetId == fromAssetId && ap.QuotingAssetId == toAssetId));
            if (assetPair == null) { return null; }

            if (!feedData.TryGetValue(assetPair.Id, out var feed)) { return null; }

            var mid = (decimal)(feed.Ask + feed.Bid) / 2;
            return assetPair.BaseAssetId == toAssetId ? 1 / mid : mid;
        }
    }
}
