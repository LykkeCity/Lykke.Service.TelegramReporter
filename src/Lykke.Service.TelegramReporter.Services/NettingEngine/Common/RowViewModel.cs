using System.Collections.Generic;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Common
{
    public class RowViewModel
    {
        public ItemViewModel Asset { get; set; }

        public string HedgeMode { get; set; }

        public IReadOnlyList<ExchangeSummaryViewModel> Exchanges { get; set; }

        public ExchangeSummaryViewModel Summary { get; set; }
    }
}
