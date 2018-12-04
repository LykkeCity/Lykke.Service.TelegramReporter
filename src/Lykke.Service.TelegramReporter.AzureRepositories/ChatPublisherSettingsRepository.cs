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

        public static string GeneratePartitionKeyForWalletsRebalancer()
        {
            return "WalletsRebalancerChatPublisher";
        }

        public static string GeneratePartitionKeyForMarketMakerArbitrages()
        {
            return "MarketMakerArbitragesChatPublisher";
        }

        public static string GeneratePartitionKeyForLiquidityEngineTrades()
        {
            return "LiquidityEngineTradesChatPublisher";
        }

        public static string GeneratePartitionKeyForLiquidityEngineSummary()
        {
            return "LiquidityEngineSummaryChatPublisher";
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

        public static ChatPublisherSettingsEntity CreateForWalletsRebalancer(IChatPublisherSettings chatPublisher)
        {
            return new ChatPublisherSettingsEntity
            {
                PartitionKey = GeneratePartitionKeyForWalletsRebalancer(),
                RowKey = Guid.NewGuid().ToString(),
                TimeSpan = chatPublisher.TimeSpan,
                ChatId = chatPublisher.ChatId
            };
        }

        public static ChatPublisherSettingsEntity CreateForMarketMakerArbitrages(IChatPublisherSettings chatPublisher)
        {
            return new ChatPublisherSettingsEntity
            {
                PartitionKey = GeneratePartitionKeyForMarketMakerArbitrages(),
                RowKey = Guid.NewGuid().ToString(),
                TimeSpan = chatPublisher.TimeSpan,
                ChatId = chatPublisher.ChatId
            };
        }

        public static ChatPublisherSettingsEntity CreateForLiquidityEngineTrades(IChatPublisherSettings chatPublisher)
        {
            return new ChatPublisherSettingsEntity
            {
                PartitionKey = GeneratePartitionKeyForLiquidityEngineTrades(),
                RowKey = Guid.NewGuid().ToString(),
                TimeSpan = chatPublisher.TimeSpan,
                ChatId = chatPublisher.ChatId
            };
        }

        public static ChatPublisherSettingsEntity CreateForLiquidityEngineSummary(IChatPublisherSettings chatPublisher)
        {
            return new ChatPublisherSettingsEntity
            {
                PartitionKey = GeneratePartitionKeyForLiquidityEngineSummary(),
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

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetWalletsRebalancerChatPublisherSettings()
        {
            return (await _storage.GetDataAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForWalletsRebalancer())).ToArray();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetMarketMakerArbitragesChatPublisherSettings()
        {
            return (await _storage.GetDataAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForMarketMakerArbitrages())).ToArray();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetLiquidityEngineTradesChatPublisherSettings()
        {
            return (await _storage.GetDataAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForLiquidityEngineTrades())).ToArray();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetLiquidityEngineSummaryChatPublisherSettings()
        {
            return (await _storage.GetDataAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForLiquidityEngineSummary())).ToArray();
        }

        public Task AddNeChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher)
        {
            var entity = ChatPublisherSettingsEntity.CreateForNe(chatPublisher);
            return _storage.InsertOrReplaceAsync(entity);
        }

        public Task AddBalanceChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher)
        {
            var entity = ChatPublisherSettingsEntity.CreateForBalance(chatPublisher);
            return _storage.InsertOrReplaceAsync(entity);
        }

        public Task AddExternalBalanceChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher)
        {
            var entity = ChatPublisherSettingsEntity.CreateForExternalBalance(chatPublisher);
            return _storage.InsertOrReplaceAsync(entity);
        }

        public Task AddWalletsRebalancerChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher)
        {
            var entity = ChatPublisherSettingsEntity.CreateForWalletsRebalancer(chatPublisher);
            return _storage.InsertOrReplaceAsync(entity);
        }

        public Task AddMarketMakerArbitragesChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher)
        {
            var entity = ChatPublisherSettingsEntity.CreateForMarketMakerArbitrages(chatPublisher);
            return _storage.InsertOrReplaceAsync(entity);
        }

        public Task AddLiquidityEngineTradesChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher)
        {
            var entity = ChatPublisherSettingsEntity.CreateForLiquidityEngineTrades(chatPublisher);
            return _storage.InsertOrReplaceAsync(entity);
        }

        public Task AddLiquidityEngineSummaryChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher)
        {
            var entity = ChatPublisherSettingsEntity.CreateForLiquidityEngineSummary(chatPublisher);
            return _storage.InsertOrReplaceAsync(entity);
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

        public async Task RemoveWalletsRebalancerChatPublisherSettingsAsync(string chatPublisherId)
        {
            await _storage.DeleteAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForWalletsRebalancer(), chatPublisherId);
        }

        public async Task RemoveMarketMakerArbitragesChatPublisherSettingsAsync(string chatPublisherId)
        {
            await _storage.DeleteAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForMarketMakerArbitrages(), chatPublisherId);
        }

        public async Task RemoveLiquidityEngineTradesChatPublisherSettingsAsync(string chatPublisherId)
        {
            await _storage.DeleteAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForLiquidityEngineTrades(), chatPublisherId);
        }

        public async Task RemoveLiquidityEngineSummaryChatPublisherSettingsAsync(string chatPublisherId)
        {
            await _storage.DeleteAsync(ChatPublisherSettingsEntity.GeneratePartitionKeyForLiquidityEngineSummary(), chatPublisherId);
        }
    }
}
