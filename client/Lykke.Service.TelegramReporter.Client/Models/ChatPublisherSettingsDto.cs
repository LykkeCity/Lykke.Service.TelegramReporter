using System;

namespace Lykke.Service.TelegramReporter.Client.Models
{
    public class ChatPublisherSettingsDto
    {
        public string ChatPublisherSettingsId { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public long ChatId { get; set; }
    }
}
