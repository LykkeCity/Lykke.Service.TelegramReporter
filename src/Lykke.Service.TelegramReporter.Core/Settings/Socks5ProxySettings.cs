using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.TelegramReporter.Core.Settings
{
    public class Socks5ProxySettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
