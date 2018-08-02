namespace Lykke.Service.TelegramReporter.Rabbit
{
    public class ExchangeSettings
    {
        public string ConnectionString { get; set; }

        public string Exchange { get; set; }

        public string QueueSuffix { get; set; }
    }
}
