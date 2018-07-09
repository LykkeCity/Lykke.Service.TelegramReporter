using System;
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
                sb.AppendLine($"======= {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} =======\r\n\r\n" +
                              "Balance Warning:\r\n\r\n" +
                              $"Wallet: {balanceIssue.ClientId}\r\n" +
                              $"Asset: {balanceIssue.AssetId}\r\n" +
                              $"Balance: {balanceIssue.Balance:0.000}\r\n" +
                              $"MinBalance: {balanceIssue.MinBalance:0.000}");
                sb.AppendLine();
            }

            return Task.FromResult(ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(sb.ToString()));
        }
    }
}
