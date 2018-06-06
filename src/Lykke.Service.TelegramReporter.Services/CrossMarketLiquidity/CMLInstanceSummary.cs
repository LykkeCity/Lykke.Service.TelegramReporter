namespace Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity
{
    public class CMLInstanceSummary
    {
        public string InstanceId { get; set; }
        public string AbsoluteInventory { get; set; }
        public string VolumeTotal { get; set; }
        public string RealizedPnL { get; set; }
        public string UnrealizedPnL { get; set; }
        public string LykkeBalances { get; set; }
        public string ExternalBalances { get; set; }
    }
}
