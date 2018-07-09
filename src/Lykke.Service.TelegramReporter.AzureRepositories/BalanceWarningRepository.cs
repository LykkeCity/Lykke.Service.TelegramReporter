using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.AzureRepositories
{
    public class BalanceWarningEntity : AzureTableEntity, IBalanceWarning
    {
        public string ClientId => PartitionKey;
        public string AssetId => RowKey;
        public decimal MinBalance { get; set; }        

        public static BalanceWarningEntity CreateForBalance(IBalanceWarning balanceWarning)
        {
            return new BalanceWarningEntity
            {
                PartitionKey = balanceWarning.ClientId,
                RowKey = balanceWarning.AssetId,
                MinBalance = balanceWarning.MinBalance,
            };
        }
    }

    public class BalanceWarningRepository : IBalanceWarningRepository
    {
        private readonly INoSQLTableStorage<BalanceWarningEntity> _storage;

        public BalanceWarningRepository(INoSQLTableStorage<BalanceWarningEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyList<IBalanceWarning>> GetBalancesWarnings()
        {
            return (await _storage.GetDataAsync()).ToArray();
        }

        public Task AddBalanceWarningAsync(IBalanceWarning balanceWarning)
        {
            var entity = BalanceWarningEntity.CreateForBalance(balanceWarning);
            return _storage.InsertAsync(entity);
        }

        public async Task RemoveBalanceWarningAsync(string clientId, string assetId)
        {
            await _storage.DeleteAsync(clientId, assetId);
        }
    }
}
