namespace Lykke.Service.TelegramReporter.Models
{
    public class ExternalBalanceWarningPost
    {
        public string Exchange { get; set; }
        public string AssetId { get; set; }
        public string Name { get; set; }
        public decimal MinBalance { get; set; }
    }
}
