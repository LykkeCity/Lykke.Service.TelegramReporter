using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Services.Balance;

namespace Lykke.Service.TelegramReporter.Services.Balance
{
    public class ExternalBalanceWarningProvider : IExternalBalanceWarningProvider
    {
        public Task<string> GetWarningMessageAsync(IList<ExternalBalanceIssueDto> balancesWithIssues)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"======= {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} =======\r\n\r\n" +
                          "External Balances Warnings:\r\n");

            foreach (var balanceIssue in balancesWithIssues)
            {
                sb.AppendLine($"Exchange: {balanceIssue.Exchange}\r\n" +
                              $"Name: {balanceIssue.Name}\r\n" +
                              $"Asset: {(!string.IsNullOrWhiteSpace(balanceIssue.AssetName) ? balanceIssue.AssetName : balanceIssue.AssetId)}\r\n" +
                              $"Balance: {balanceIssue.Balance:0.000}\r\n" +
                              $"Min Balance: {balanceIssue.MinBalance:0.000}");
                sb.AppendLine();
            }

            return Task.FromResult(ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(sb.ToString()));
        }
    }
}
