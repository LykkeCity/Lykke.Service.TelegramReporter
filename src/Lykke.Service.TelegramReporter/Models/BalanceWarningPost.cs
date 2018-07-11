namespace Lykke.Service.TelegramReporter.Models
{
    public class BalanceWarningPost
    {
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public string Name { get; set; }
        public decimal MinBalance { get; set; }
    }
}
