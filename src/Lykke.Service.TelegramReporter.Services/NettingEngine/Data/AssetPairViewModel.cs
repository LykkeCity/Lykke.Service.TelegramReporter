namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Data
{
    public class AssetPairViewModel
    {
        public string Id { get; set; }

        public bool Active { get; set; }

        public decimal PnL { get; set; }

        public decimal SellVolumeCoefficient { get; set; }

        public decimal BuyVolumeCoefficient { get; set; }

        public decimal QuoteAssetBalance { get; set; }

        public InstrumentSettingsViewModel Settings { get; set; }

        public QuoteViewModel LykkeQuote { get; set; }

        public QuoteViewModel ExternalQuote { get; set; }
    }
}
