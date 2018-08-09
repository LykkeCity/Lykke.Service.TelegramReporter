using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.AzureRepositories
{
    public class ExternalBalanceWarningEntity : AzureTableEntity, IExternalBalanceWarning
    {
        public string Exchange => PartitionKey;
        public string AssetId => RowKey;
        public string Name { get; set; }
        public decimal MinBalance { get; set; }

        public static ExternalBalanceWarningEntity CreateForBalance(IExternalBalanceWarning balanceWarning)
        {
            return new ExternalBalanceWarningEntity
            {
                PartitionKey = balanceWarning.Exchange,
                RowKey = balanceWarning.AssetId.ToUpperInvariant(),
                Name = balanceWarning.Name,
                MinBalance = balanceWarning.MinBalance,
            };
        }
    }

    public class ExternalBalanceWarningRepository : IExternalBalanceWarningRepository
    {
        private readonly INoSQLTableStorage<ExternalBalanceWarningEntity> _storage;

        public ExternalBalanceWarningRepository(INoSQLTableStorage<ExternalBalanceWarningEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyList<IExternalBalanceWarning>> GetExternalBalancesWarnings()
        {
            return (await _storage.GetDataAsync()).ToArray();
        }

        public Task AddExternalBalanceWarningAsync(IExternalBalanceWarning balanceWarning)
        {
            var entity = ExternalBalanceWarningEntity.CreateForBalance(balanceWarning);
            return _storage.InsertOrReplaceAsync(entity);
        }

        public async Task RemoveExternalBalanceWarningAsync(string exchange, string assetId)
        {
            await _storage.DeleteAsync(exchange, assetId.ToUpperInvariant());
        }
    }
}
