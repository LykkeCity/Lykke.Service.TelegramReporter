using System;
using JetBrains.Annotations;
using Lykke.Service.TelegramReporter.Core.Settings;

namespace Lykke.Service.TelegramReporter.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class TelegramReporterSettings
    {
        public DbSettings Db { get; set; }

        public TelegramSettings Telegram { get; set; }

        public TimeSpan AssetsCacheExpirationPeriod { get; set; }
    }
}
