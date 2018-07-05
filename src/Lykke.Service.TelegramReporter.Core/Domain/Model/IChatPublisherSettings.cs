using System;

namespace Lykke.Service.TelegramReporter.Core.Domain.Model
{
    public interface IChatPublisherSettings
    {
        string ChatPublisherSettingsId { get; }
        TimeSpan TimeSpan { get; }
        long ChatId { get; }
    }

    public class ChatPublisherSettings : IChatPublisherSettings
    {
        public string ChatPublisherSettingsId { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public long ChatId { get; set; }
    }
}
