using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.TelegramReporter.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public TelegramReporterSettings TelegramReporterService { get; set; }

        public CrossMarketLiquidityServiceClientInstancesSettings CrossMarketLiquidityServiceClient { get; set; }
    }
}
