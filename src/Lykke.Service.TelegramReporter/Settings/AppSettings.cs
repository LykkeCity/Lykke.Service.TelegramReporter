using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.NettingEngine.Client;
using Lykke.Service.SpreadEngine.Client;

namespace Lykke.Service.TelegramReporter.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public TelegramReporterSettings TelegramReporterService { get; set; }

        public CrossMarketLiquidityServiceClientInstancesSettings CrossMarketLiquidityServiceClient { get; set; }
        public AssetsServiceClientSettings AssetsServiceClient { get; set; }
        public BalanceServiceClientSettings BalancesServiceClient { get; set; }
        public RateCalculatorServiceClientSettings RateCalculatorServiceClient { get; set; }
        public SpreadEngineServiceClientSettings SpreadEngineServiceClient { get; set; }
        public NettingEngineServiceClientSettings NettingEngineServiceClient { get; set; }
    }

    public class AssetsServiceClientSettings
    {
        public string ServiceUrl { get; set; }
    }

    public class BalanceServiceClientSettings
    {
        public string ServiceUrl { get; set; }
    }

    public class RateCalculatorServiceClientSettings
    {
        public string ServiceUrl { get; set; }
    }
}
