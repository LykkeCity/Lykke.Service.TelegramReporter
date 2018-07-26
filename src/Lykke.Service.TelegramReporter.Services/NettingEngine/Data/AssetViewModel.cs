namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Data
{
    public class AssetViewModel
    {
        public string Id { get; set; }

        public decimal Balance { get; set; }

        public InstrumentSettingsViewModel Settings { get; set; }
    }
}
