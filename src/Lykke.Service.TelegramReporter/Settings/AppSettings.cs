using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.MarketMakerArbitrageDetector.Client;
using Lykke.Service.MarketMakerReports.Client;
using Lykke.Service.NettingEngine.Client;

namespace Lykke.Service.TelegramReporter.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public TelegramReporterSettings TelegramReporterService { get; set; }

        public AssetsServiceClientSettings AssetsServiceClient { get; set; }
        public BalanceServiceClientSettings BalancesServiceClient { get; set; }
        public MarketMakerReportsServiceClientSettings MarketMakerReportsServiceClient { get; set; }
        public MarketMakerReportsServiceClientSettings FiatMarketMakerReportsServiceClient { get; set; }
        public NettingEngineServiceClientSettings NettingEngineServiceClient { get; set; }
        public MarketMakerArbitrageDetectorServiceClientSettings MarketMakerArbitrageDetectorServiceClient { get; set; }
        public LiquidityEngineServiceClientInstancesSettings LiquidityEngineServiceClient { get; set; }
        public CryptoIndexServiceClientInstancesSettings CryptoIndexServiceClient { get; set; }
        public DwhServiceClientSettings DwhServiceClient { get; set; }
    }

    public class AssetsServiceClientSettings
    {
        public string ServiceUrl { get; set; }
    }

    public class BalanceServiceClientSettings
    {
        public string ServiceUrl { get; set; }
    }

    public class DwhServiceClientSettings
    {
        public string ServiceUrl { get; set; }
    }
}
