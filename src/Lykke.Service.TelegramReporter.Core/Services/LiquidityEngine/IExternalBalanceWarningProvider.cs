using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Core.Services.LiquidityEngine
{
    public interface IExternalBalanceWarningProvider
    {
        Task<string> GetWarningMessageAsync(IList<ExternalBalanceIssueDto> balancesWithIssues);
    }
}
