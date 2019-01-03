using System;

namespace Lykke.Service.TelegramReporter.Client.Models
{
    /// <summary>
    /// Channel reporter data transfer object
    /// </summary>
    public class ReportChannelDto
    {
        /// <summary>
        /// create empty
        /// </summary>
        public ReportChannelDto()
        {
        }

        /// <summary>
        /// create changel
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="type"></param>
        /// <param name="chatId"></param>
        /// <param name="interval"></param>
        /// <param name="metainfo"></param>
        public ReportChannelDto(string channelId, string type, long chatId, TimeSpan interval, string metainfo)
        {
            ChannelId = channelId;
            Type = type;
            ChatId = chatId;
            Interval = interval;
            Metainfo = metainfo;
        }

        public string ChannelId { get; set; }
        public string Type { get; set; }
        public long ChatId { get; set; }
        public TimeSpan Interval { get; set; }
        public string Metainfo { get; set; }
    }
}
