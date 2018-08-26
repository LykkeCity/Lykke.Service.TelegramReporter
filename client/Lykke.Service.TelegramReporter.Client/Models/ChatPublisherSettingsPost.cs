using System;

namespace Lykke.Service.TelegramReporter.Client.Models
{
    public class ChatPublisherSettingsPost
    {
        public TimeSpan TimeSpan { get; set; }
        public long ChatId { get; set; }
    }
}
