using System.Collections.Generic;
using Lykke.Service.SpreadEngine.Client.Api;

namespace Lykke.Service.TelegramReporter.Core.Instances
{
    public class SpreadEngineInstance
    {
        public SpreadEngineInstance(int index)
        {
            Index = index;
        }

        public int Index { get; }

        public string DisplayName { get; set; }

        public IReadOnlyList<string> Exchanges { get; set; }

        public ISettingsApi Settings { get; set; }

        public IInstrumentsApi Instruments { get; set; }

        public ITradersApi Traders { get; set; }

        public IOrderBooksApi OrderBooks { get; set; }

        public IBalancesApi Balances { get; set; }
    }
}
