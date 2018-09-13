using System.Collections.Generic;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Common
{
    public class InventoryReportViewModel
    {
        public IReadOnlyList<string> Exchanges { get; set; }

        public IReadOnlyList<RowViewModel> Rows { get; set; }

        public TotalRowViewModel Total { get; set; }

        public bool Extended { get; set; }
    }
}
