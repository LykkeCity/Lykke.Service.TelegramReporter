using System.Collections.Generic;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Common
{
    public class TotalRowViewModel
    {
        public IReadOnlyList<ExchangeTotalViewModel> Exchanges { get; set; }
    }
}
