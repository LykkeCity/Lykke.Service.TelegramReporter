using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.TelegramReporter.Core.Services.Channelv2;

namespace Lykke.Service.TelegramReporter.AzureRepositories
{
    public class ReportChannelEntity : AzureTableEntity, IReportChannel
    {
        public static string GeneratePartitionKey() => "Channels";
        public static string GenerateRowKey(string channelId) => channelId;

        public ReportChannelEntity()
        {
        }

        public ReportChannelEntity(IReportChannel channel)
            :this(channel.ChannelId, channel.Type, channel.ChatId, channel.Interval, channel.Metainfo)
        {
        }

        public ReportChannelEntity(string channelId, string type, long chatId, TimeSpan interval, string metainfo)
        {
            ChannelId = channelId;
            Type = type;
            ChatId = chatId;
            Interval = interval;
            Metainfo = metainfo;

            PartitionKey = GeneratePartitionKey();
            RowKey = GenerateRowKey(channelId);
        }

        public string ChannelId { get; set; }
        public string Type { get; set; }
        public long ChatId { get; set; }
        public TimeSpan Interval { get; set; }
        public string Metainfo { get; set; }
    }

    public class ChannelRepository : IChannelRepository
    {
        private readonly INoSQLTableStorage<ReportChannelEntity> _storage;

        public ChannelRepository(INoSQLTableStorage<ReportChannelEntity> storage)
        {
            _storage = storage;
        }

        public Task AddOrUpdateChannelAsync(IReportChannel channel)
        {
            var entity = new ReportChannelEntity(channel);
            return _storage.InsertOrReplaceAsync(entity);
        }

        public Task RemoveChannelAsync(string channelId)
        {
            return _storage.DeleteIfExistAsync(ReportChannelEntity.GeneratePartitionKey(), ReportChannelEntity.GenerateRowKey(channelId));
        }

        public async Task<List<IReportChannel>> GetAllChannelsAsync()
        {
            var data = await _storage.GetDataAsync(ReportChannelEntity.GeneratePartitionKey());
            return data.Cast<IReportChannel>().ToList();
        }
    }
}
