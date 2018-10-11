namespace Lykke.Service.TelegramReporter.Settings
{
    public class LiquidityEngineServiceClientInstancesSettings
    {
        public LiquidityEngineClientSettings[] Instances { get; set; }
    }

    public class LiquidityEngineClientSettings
    {
        public string DisplayName { get; set; }

        public string ServiceUrl { get; set; }
    }
}
