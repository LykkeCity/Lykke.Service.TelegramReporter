using System;

namespace Lykke.Service.TelegramReporter.Models
{
    public class ChatPublisherSettingsPost
    {
        public TimeSpan TimeSpan { get; set; }
        public long ChatId { get; set; }
    }
}
