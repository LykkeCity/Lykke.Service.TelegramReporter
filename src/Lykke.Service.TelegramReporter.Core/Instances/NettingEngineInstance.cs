﻿using Lykke.Service.NettingEngine.Client.Api;

namespace Lykke.Service.TelegramReporter.Core.Instances
{
    public class NettingEngineInstance
    {
        public NettingEngineInstance(int index)
        {
            Index = index;
        }

        public int Index { get; }

        public string DisplayName { get; set; }

        public IAccountSettingsApi AccountSettings { get; set; }

        public IBalancesApi Balances { get; set; }

        public IDailyInventoryApi DailyInventory { get; set; }

        public IInstrumentInventoryApi InstrumentInventory { get; set; }

        public IInstrumentsApi Instruments { get; set; }

        public IInstrumentSettingsApi InstrumentSettings { get; set; }

        public IInventoryApi Inventory { get; set; }

        public IOrderBooksApi OrderBooks { get; set; }

        public ISettingsApi Settings { get; set; }

        public ITradesApi Trades { get; set; }

        public IAssetHedgeSettingsApi AssetHedgeSettings { get; set; }

        public IExchangesApi Exchanges { get; set; }

        public IServiceInfoApi ServiceInfo { get; set; }

        public IExternalInstrumentsApi ExternalInstruments { get; set; }

        public IHedgeLimitOrdersApi HedgeLimitOrders { get; set; }
    }
}
