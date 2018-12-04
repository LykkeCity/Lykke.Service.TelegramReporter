namespace Lykke.Service.TelegramReporter.Services.LiquidityEngine
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
