using System;

namespace Lykke.Service.TelegramReporter.Core.Services.Channelv2
{
    public interface IReportChannel
    {
        string ChannelId { get; }
        string Type { get; }
        long ChatId { get; }
        TimeSpan Interval { get; }
        string Metainfo { get; }
    }
}
