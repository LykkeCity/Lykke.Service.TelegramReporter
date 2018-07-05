using System;

namespace Lykke.Service.TelegramReporter.Models
{
    public class ChatPublisherSettingsDto
    {
        public string ChatPublisherSettingsId { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public long ChatId { get; set; }
    }
}
