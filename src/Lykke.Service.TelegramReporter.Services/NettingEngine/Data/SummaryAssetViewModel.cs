namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Data
{
    public class SummaryAssetViewModel
    {
        public string Asset { get; set; }
        public decimal AbsoluteInventory { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalSell { get; set; }
        public decimal TotalBuy { get; set; }
        public decimal? TotalPnL { get; set; }
    }
}
