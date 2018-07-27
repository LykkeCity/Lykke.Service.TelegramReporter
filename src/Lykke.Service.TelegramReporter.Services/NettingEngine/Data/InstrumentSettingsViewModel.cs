using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Data
{
    public class InstrumentSettingsViewModel
    {
        public decimal? TargetSpread { get; set; }

        public decimal? BestLevelVolume { get; set; }

        public int? Depth { get; set; }

        public decimal? SideSumVolume { get; set; }

        public decimal? AlphaVolumeFactor { get; set; }

        public decimal? SpreadScalingFactor { get; set; }

        public string ExternalExchange { get; set; }

        public decimal? ThresholdFractionToCorrectOrders { get; set; }

        public IReadOnlyList<ItemViewModel> AllowArbitrage { get; set; }

        public IReadOnlyList<ItemViewModel> TraderTypes { get; set; }

        public ItemViewModel SelectedArbitrage
            => AllowArbitrage?.FirstOrDefault(o => o.Selected);

        public ItemViewModel SelectedTraderType
            => TraderTypes?.FirstOrDefault(o => o.Selected);
    }
}
