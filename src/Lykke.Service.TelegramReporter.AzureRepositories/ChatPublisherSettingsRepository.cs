using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.AzureRepositories
{
    public class ChatPublisherSettingsEntity : AzureTableEntity, IChatPublisherSettings
    {
        public string ChatPublisherSettingsId => RowKey;
        public TimeSpan TimeSpan { get; set; }
        public long ChatId { get; set; }

        public static string GeneratePartitionKeyForCml()
        {
            return "CmlChatPublisher";
        }

        public static string GeneratePartitionKeyForSe()
        {
            return "SeChatPublisher";
        }

        public static ChatPublisherSettingsEntity CreateForCml(IChatPublisherSettings chatPublisher)
        {
            return new ChatPublisherSettingsEntity
            {
                PartitionKey = GeneratePartitionKeyForCml(),
                RowKey = Guid.NewGuid().ToString(),
                TimeSpan = chatPublisher.TimeSpan,
                ChatId = chatPublisher.ChatId
            };
        }

        public static ChatPublisherSettingsEntity CreateForSe(IChatPublisherSettings chatPublisher)
        {
            return new ChatPublisherSettingsEntity
            {
                PartitionKey = GeneratePartitionKeyForSe(),
                RowKey = Guid.NewGuid().ToString(),
                TimeSpan = chatPublisher.TimeSpan,
                ChatId = chatPublisher.ChatId
            };
        }
    }

    public class ChatPublisherSettingsRepository : IChatPublisherSettingsRepository
    {
        private readonly INoSQLTableStorage<ChatPublisherSettingsEntity> _storage;

        public ChatPublisherSettingsRepository(INoSQLTableStorage<ChatPublisherSettingsEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetCmlChatPublisherSettings()
        {
            return (await _storage.GetDataAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForCml())).ToArray();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetSeChatPublisherSettings()
        {
            return (await _storage.GetDataAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForSe())).ToArray();
        }

        public Task AddCmlChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher)
        {
            var entity = ChatPublisherSettingsEntity.CreateForCml(chatPublisher);
            return _storage.InsertAsync(entity);
        }

        public Task AddSeChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher)
        {
            var entity = ChatPublisherSettingsEntity.CreateForSe(chatPublisher);
            return _storage.InsertAsync(entity);
        }

        public async Task RemoveCmlChatPublisherSettingsAsync(string chatPublisherSettingsId)
        {
            await _storage.DeleteAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForCml(), chatPublisherSettingsId);
        }

        public async Task RemoveSeChatPublisherSettingsAsync(string chatPublisherId)
        {
            await _storage.DeleteAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForSe(), chatPublisherId);
        }
    }
}
