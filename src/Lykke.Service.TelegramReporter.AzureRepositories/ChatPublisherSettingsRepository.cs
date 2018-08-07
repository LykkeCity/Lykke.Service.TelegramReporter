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

        public static string GeneratePartitionKeyForBalance()
        {
            return "BalanceChatPublisher";
        }

        public static string GeneratePartitionKeyForExternalBalance()
        {
            return "ExternalBalanceChatPublisher";
        }

        public static string GeneratePartitionKeyForNe()
        {
            return "NeChatPublisher";
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

        public static ChatPublisherSettingsEntity CreateForNe(IChatPublisherSettings chatPublisher)
        {
            return new ChatPublisherSettingsEntity
            {
                PartitionKey = GeneratePartitionKeyForNe(),
                RowKey = Guid.NewGuid().ToString(),
                TimeSpan = chatPublisher.TimeSpan,
                ChatId = chatPublisher.ChatId
            };
        }

        public static ChatPublisherSettingsEntity CreateForBalance(IChatPublisherSettings chatPublisher)
        {
            return new ChatPublisherSettingsEntity
            {
                PartitionKey = GeneratePartitionKeyForBalance(),
                RowKey = Guid.NewGuid().ToString(),
                TimeSpan = chatPublisher.TimeSpan,
                ChatId = chatPublisher.ChatId
            };
        }

        public static ChatPublisherSettingsEntity CreateForExternalBalance(IChatPublisherSettings chatPublisher)
        {
            return new ChatPublisherSettingsEntity
            {
                PartitionKey = GeneratePartitionKeyForExternalBalance(),
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

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetNeChatPublisherSettings()
        {
            return (await _storage.GetDataAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForNe())).ToArray();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetBalanceChatPublisherSettings()
        {
            return (await _storage.GetDataAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForBalance())).ToArray();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetExternalBalanceChatPublisherSettings()
        {
            return (await _storage.GetDataAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForExternalBalance())).ToArray();
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

        public Task AddNeChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher)
        {
            var entity = ChatPublisherSettingsEntity.CreateForNe(chatPublisher);
            return _storage.InsertAsync(entity);
        }

        public Task AddBalanceChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher)
        {
            var entity = ChatPublisherSettingsEntity.CreateForBalance(chatPublisher);
            return _storage.InsertAsync(entity);
        }

        public Task AddExternalBalanceChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher)
        {
            var entity = ChatPublisherSettingsEntity.CreateForExternalBalance(chatPublisher);
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

        public async Task RemoveNeChatPublisherSettingsAsync(string chatPublisherId)
        {
            await _storage.DeleteAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForNe(), chatPublisherId);
        }

        public async Task RemoveBalanceChatPublisherSettingsAsync(string chatPublisherId)
        {
            await _storage.DeleteAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForBalance(), chatPublisherId);
        }

        public async Task RemoveExternalBalanceChatPublisherSettingsAsync(string chatPublisherId)
        {
            await _storage.DeleteAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForExternalBalance(), chatPublisherId);
        }
    }
}
