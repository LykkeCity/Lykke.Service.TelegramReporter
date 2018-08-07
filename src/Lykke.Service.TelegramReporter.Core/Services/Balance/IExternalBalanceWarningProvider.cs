using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Core.Services.Balance
{
    public interface IExternalBalanceWarningProvider
    {
        Task<string> GetWarningMessageAsync(IList<ExternalBalanceIssueDto> balancesWithIssues);
    }
}
