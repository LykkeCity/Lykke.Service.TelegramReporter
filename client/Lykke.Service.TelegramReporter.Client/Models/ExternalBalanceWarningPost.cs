namespace Lykke.Service.TelegramReporter.Client.Models
{
    public class ExternalBalanceWarningPost
    {
        public string Exchange { get; set; }
        public string AssetId { get; set; }
        public string Name { get; set; }
        public string AssetName { get; set; }
        public decimal MinBalance { get; set; }
    }
}
