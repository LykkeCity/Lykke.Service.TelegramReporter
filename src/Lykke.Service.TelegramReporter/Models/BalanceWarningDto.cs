namespace Lykke.Service.TelegramReporter.Models
{
    public class BalanceWarningDto
    {
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public decimal MinBalance { get; set; }
    }
}
