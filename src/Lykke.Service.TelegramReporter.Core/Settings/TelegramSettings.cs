using System;

namespace Lykke.Service.TelegramReporter.Core.Settings
{
    public class TelegramSettings
    {
        public Socks5ProxySettings ProxySettings { get; set; }

        public string TelegramToken { get; set; }
    }
}
