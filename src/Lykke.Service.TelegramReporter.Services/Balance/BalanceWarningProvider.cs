using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Services.Balance;

namespace Lykke.Service.TelegramReporter.Services.Balance
{
    public class BalanceWarningProvider : IBalanceWarningProvider
    {
        public Task<string> GetWarningMessageAsync(IList<BalanceIssueDto> balancesWithIssues)
        {
            var sb = new StringBuilder();

            foreach (var balanceIssue in balancesWithIssues)
            {
                sb.AppendLine($"Wallet: {balanceIssue.ClientId} " +
                              $"Asset: {balanceIssue.AssetId} " +
                              $"Balance: {balanceIssue.Balance:0.000} " +
                              $"MinBalance: {balanceIssue.MinBalance:0.000}");
                sb.AppendLine();
            }

            return Task.FromResult(ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(sb.ToString()));
        }
    }
}
