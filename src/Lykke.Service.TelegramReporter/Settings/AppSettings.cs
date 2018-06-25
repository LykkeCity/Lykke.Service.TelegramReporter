using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.SpreadEngine.Client;

namespace Lykke.Service.TelegramReporter.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public TelegramReporterSettings TelegramReporterService { get; set; }

        public CrossMarketLiquidityServiceClientInstancesSettings CrossMarketLiquidityServiceClient { get; set; }
        public SpreadEngineServiceClientSettings SpreadEngineServiceClient { get; set; }
    }
}
