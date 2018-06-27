using JetBrains.Annotations;
using Lykke.Service.TelegramReporter.Core.Settings;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.TelegramReporter.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class TelegramReporterSettings
    {
        public DbSettings Db { get; set; }

        public TelegramSettings Telegram { get; set; }

        public PublisherSettings CmlPublisher { get; set; }

        public PublisherSettings SpreadEnginePublisher { get; set; }
    }
}
