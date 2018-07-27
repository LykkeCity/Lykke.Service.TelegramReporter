namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Data
{
    public class AssetInventoryModel
    {
        public string AssetId { get; set; }

        public decimal Volume { get; set; }

        public decimal BaseVolume { get; set; }

        public decimal Balance { get; set; }
    }
}
