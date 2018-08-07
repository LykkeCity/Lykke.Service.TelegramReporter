namespace Lykke.Service.TelegramReporter.Core.Domain.Model
{
    public interface IExternalBalanceWarning
    {
        string Exchange { get; }
        string AssetId { get; }
        string Name { get; }
        decimal MinBalance { get; }        
    }

    public class ExternalBalanceWarning : IExternalBalanceWarning
    {
        public string Exchange { get; set; }
        public string AssetId { get; set; }
        public string Name { get; set; }
        public decimal MinBalance { get; set; }        
    }
}
