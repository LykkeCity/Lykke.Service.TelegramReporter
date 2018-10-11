namespace Lykke.Service.TelegramReporter.Services.LquidityEngineTrades
{
    public class LiquidityEngineUrlSettings
    {
        public LiquidityEngineUrlSettings(string[] urls)
        {
            Urls = urls;
        }

        public string[] Urls { get; set; }
    }
}
