namespace Lykke.Service.TelegramReporter.Models
{
    public class ExternalBalanceWarningDto
    {
        public string Exchange { get; set; }
        public string AssetId { get; set; }
        public string Name { get; set; }
        public decimal MinBalance { get; set; }
    }
}
