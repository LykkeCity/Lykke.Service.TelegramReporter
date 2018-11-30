using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Services.LiquidityEngine;

namespace Lykke.Service.TelegramReporter.Services.LiquidityEngine
{
    public class BalanceWarningProvider : IBalanceWarningProvider
    {
        public Task<string> GetWarningMessageAsync(IList<BalanceIssueDto> balancesWithIssues)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"======= {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} =======\r\n\r\n" +
                          "Balances Warnings:\r\n");

            foreach (var balanceIssue in balancesWithIssues)
            {
                sb.AppendLine($"Wallet Id: {balanceIssue.ClientId}\r\n" +
                              $"Wallet Name: {balanceIssue.Name}\r\n" +
                              $"Asset: {(!string.IsNullOrWhiteSpace(balanceIssue.AssetName) ? balanceIssue.AssetName : balanceIssue.AssetId)}\r\n" +
                              $"Balance: {balanceIssue.Balance:0.000}\r\n" +
                              $"Min Balance: {balanceIssue.MinBalance:0.000}");
                sb.AppendLine();
            }

            return Task.FromResult(ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(sb.ToString()));
        }
    }
}
