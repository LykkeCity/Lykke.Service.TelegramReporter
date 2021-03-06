﻿namespace Lykke.Service.TelegramReporter.Core.Services.LiquidityEngine
{
    public class BalanceIssueDto
    {
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public string Name { get; set; }
        public string AssetName { get; set; }
        public decimal Balance { get; set; }
        public decimal MinBalance { get; set; }
    }
}
