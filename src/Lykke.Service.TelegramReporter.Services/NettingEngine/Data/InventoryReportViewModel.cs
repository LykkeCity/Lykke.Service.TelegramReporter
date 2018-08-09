using System.Collections.Generic;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Data
{
    public class InventoryReportViewModel
    {
        public IReadOnlyList<string> Exchanges { get; set; }

        public IReadOnlyList<RowViewModel> Rows { get; set; }

        public TotalRowViewModel Total { get; set; }
    }
}
