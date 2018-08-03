using System.Collections.Generic;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Data
{
    public class TotalRowViewModel
    {
        public IReadOnlyList<ExchangeTotalViewModel> Exchanges { get; set; }
    }
}
