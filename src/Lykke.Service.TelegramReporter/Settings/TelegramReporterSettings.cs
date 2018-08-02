using System;
using JetBrains.Annotations;
using Lykke.Service.TelegramReporter.Core.Settings;
using Lykke.Service.TelegramReporter.Rabbit;

namespace Lykke.Service.TelegramReporter.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class TelegramReporterSettings
    {
        public DbSettings Db { get; set; }

        public TelegramSettings Telegram { get; set; }

        public TimeSpan AssetsCacheExpirationPeriod { get; set; }

        public NettingEngineAuditExchangeSettings NettingEngineAuditExchange { get; set; }
    }

    public class NettingEngineAuditExchangeSettings
    {
        public ExchangeSettings Exchange { get; set; }
    }
}
