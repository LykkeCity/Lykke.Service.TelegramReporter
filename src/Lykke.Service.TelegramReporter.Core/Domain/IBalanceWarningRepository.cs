using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.Core.Domain
{
    public interface IBalanceWarningRepository
    {
        Task<IReadOnlyList<IBalanceWarning>> GetBalancesWarnings();

        Task AddBalanceWarningAsync(IBalanceWarning balanceWarning);

        Task RemoveBalanceWarningAsync(string clientId, string assetId);
    }
}
