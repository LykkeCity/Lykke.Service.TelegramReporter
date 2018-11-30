using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Core.Services.LiquidityEngine
{
    public interface IBalanceWarningProvider
    {
        Task<string> GetWarningMessageAsync(IList<BalanceIssueDto> balancesWithIssues);
    }
}
