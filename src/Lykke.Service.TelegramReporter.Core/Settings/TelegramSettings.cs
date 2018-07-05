using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.TelegramReporter.Core.Settings
{
    public class TelegramSettings
    {
        [Optional]
        public Socks5ProxySettings ProxySettings { get; set; }

        public string TelegramToken { get; set; }
    }
}
