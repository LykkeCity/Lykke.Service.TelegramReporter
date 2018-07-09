using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.SpreadEngine.Client;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.TelegramReporter.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public TelegramReporterSettings TelegramReporterService { get; set; }

        public CrossMarketLiquidityServiceClientInstancesSettings CrossMarketLiquidityServiceClient { get; set; }
        public AssetsServiceClientSettings AssetsServiceClient { get; set; }
        public BalanceServiceClientSettings BalancesServiceClient { get; set; }
        public SpreadEngineServiceClientSettings SpreadEngineServiceClient { get; set; }
    }

    public class AssetsServiceClientSettings
    {
        public string ServiceUrl { get; set; }
    }

    public class BalanceServiceClientSettings
    {
        public string ServiceUrl { get; set; }
    }
}
