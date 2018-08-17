namespace Lykke.Service.TelegramReporter.Models
{
    public class BalanceWarningDto
    {
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public string Name { get; set; }
        public string AssetName { get; set; }
        public decimal MinBalance { get; set; }
    }
}
