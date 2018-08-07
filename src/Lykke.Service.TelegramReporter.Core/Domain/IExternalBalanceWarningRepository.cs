using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.Core.Domain
{
    public interface IExternalBalanceWarningRepository
    {
        Task<IReadOnlyList<IExternalBalanceWarning>> GetExternalBalancesWarnings();

        Task AddExternalBalanceWarningAsync(IExternalBalanceWarning balanceWarning);

        Task RemoveExternalBalanceWarningAsync(string exchange, string assetId);
    }
}
