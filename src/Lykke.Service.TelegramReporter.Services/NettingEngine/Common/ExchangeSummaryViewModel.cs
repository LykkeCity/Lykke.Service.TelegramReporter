namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Common
{
    public class ExchangeSummaryViewModel
    {
        public decimal Balance { get; set; }

        public decimal BalanceUsd { get; set; }

        public decimal BalanceRatio { get; set; }

        public decimal Inventory { get; set; }

        public decimal InventoryUsd { get; set; }

        public decimal Sell { get; set; }

        public decimal Buy { get; set; }
    }
}
