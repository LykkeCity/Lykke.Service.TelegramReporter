namespace Lykke.Service.TelegramReporter.Settings
{
    public class CryptoIndexServiceClientInstancesSettings
    {
        public CryptoIndexClientSettings[] Instances { get; set; }
    }

    public class CryptoIndexClientSettings
    {
        public string DisplayName { get; set; }

        public string ServiceUrl { get; set; }
    }
}
