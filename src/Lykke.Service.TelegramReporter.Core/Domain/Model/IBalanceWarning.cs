﻿namespace Lykke.Service.TelegramReporter.Core.Domain.Model
{
    public interface IBalanceWarning
    {
        string ClientId { get; }
        string AssetId { get; }
        decimal MinBalance { get; }        
    }

    public class BalanceWarning : IBalanceWarning
    {
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public decimal MinBalance { get; set; }        
    }
}