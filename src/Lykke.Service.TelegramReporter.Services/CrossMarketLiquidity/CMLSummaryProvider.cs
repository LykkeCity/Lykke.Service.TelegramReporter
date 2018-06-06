using Lykke.Service.CrossMarketLiquidity.Client.AutorestClient.Models;
using Lykke.Service.TelegramReporter.Core.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Services;

namespace Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity
{
    public class CMLSummaryProvider : ICMLSummaryProvider
    {
        private ICrossMarketLiquidityInstanceManager _crossMarketLiquidityInstanceManager;

        public CMLSummaryProvider(ICrossMarketLiquidityInstanceManager crossMarketLiquidityInstanceManager)
        {
            _crossMarketLiquidityInstanceManager = crossMarketLiquidityInstanceManager;
        }

        public async Task<string> GetSummaryMessageAsync()
        {
            return GetSummaryMessage(await GetSummaryAsync());
        }

        private async Task<CMLInstanceSummary[]> GetSummaryAsync()
        {
            var tasks = new List<Tuple<string, Task<InventoryStateDto>, Task<BalanceListDto>>>();
            foreach (var instance in _crossMarketLiquidityInstanceManager)
            {
                tasks.Add(new Tuple<string, Task<InventoryStateDto>, Task<BalanceListDto>>(instance.Key,
                    instance.Value.GetInventoryStateAsync(), instance.Value.GetBalancesAsync()));
            }

            await Task.WhenAll(tasks.Select(t => t.Item2).Cast<Task>().Union(tasks.Select(t => t.Item3)));

            var instanceModels = new List<CMLInstanceSummary>();
            foreach (var tuple in tasks)
            {
                string instance = tuple.Item1;
                var inventoryState = tuple.Item2.Result;
                var balances = tuple.Item3.Result;
                instanceModels.Add(GetInstanceSummary(instance, inventoryState, balances));
            }

            return instanceModels.ToArray();
        }

        private CMLInstanceSummary GetInstanceSummary(string instanceId, InventoryStateDto inventoryState,
            BalanceListDto balances)
        {
            return new CMLInstanceSummary
            {
                InstanceId = instanceId,
                AbsoluteInventory = (inventoryState?.AbsoluteInventory ?? 0).ToString(),
                VolumeTotal =
                    (inventoryState == null
                        ? 0
                        : (inventoryState.RealizedBuyVolumeTotal + inventoryState.RealizedSellVolumeTotal)).ToString(),
                RealizedPnL = (inventoryState?.RealizedPnL ?? 0).ToString("0.####"),
                UnrealizedPnL = (inventoryState?.LykkeUnrealizedPnl ?? 0).ToString("0.####"),
                LykkeBalances = GetBalancesValue(balances?.LykkeBalance),
                ExternalBalances = GetBalancesValue(balances?.ExternalBalance)
            };
        }

        private string GetBalancesValue(AssetPairBalancesDto balances)
        {
            if (balances == null)
            {
                return "No balances";
            }

            return $"{balances.Asset.BaseAsset}:{balances.BaseAssetBalance}" +
                $" {balances.Asset.QuoteAsset}:{balances.QuoteAssetBalance}";
        }


        private string GetSummaryMessage(IEnumerable<CMLInstanceSummary> summary)
        {
            const string instanceIdHeader = "Name";
            const string inventoryHeader = "Inventory";
            const string volumeTotalHeader = "VolumeTotal";
            const string realizedPnLHeader = "RealizedPnL";
            const string unrealizedPnLHeader = "UnrealizedPnL";
            const string lykkeBalancesHeader = "Lykke Balances";
            const string externalBalancesHeader = "External Balances";

            var sb = new StringBuilder("<pre>");
            foreach (var instanceSummary in summary)
            {
                string row = $"{instanceIdHeader}: {instanceSummary.InstanceId}\r\n" +
                             $"{inventoryHeader}: {instanceSummary.AbsoluteInventory}\r\n" +
                             $"{volumeTotalHeader}: {instanceSummary.VolumeTotal}\r\n" +
                             $"{realizedPnLHeader}: {instanceSummary.RealizedPnL}\r\n" +
                             $"{unrealizedPnLHeader}: {instanceSummary.UnrealizedPnL}\r\n" +
                             $"{lykkeBalancesHeader}: {instanceSummary.LykkeBalances}\r\n" +
                             $"{externalBalancesHeader}: {instanceSummary.ExternalBalances}";
                sb.AppendLine(row);
                sb.AppendLine();
            }
            sb.Append("</pre>");
            return sb.ToString();
        }

        private string GetSummaryMessage1(IEnumerable<CMLInstanceSummary> summary)
        {
            const string instanceIdHeader = "Name";
            int instanceIdMaxLength = summary.Max(s => s.InstanceId.Length);
            int instanceIdColumnLength = GetColumnLength(instanceIdHeader, instanceIdMaxLength);

            const string inventoryHeader = "Inventory";
            int inventoryMaxLength = summary.Max(s => s.AbsoluteInventory.Length);
            int inventoryColumnLength = GetColumnLength(inventoryHeader, inventoryMaxLength);

            const string volumeTotalHeader = "VolumeTotal";
            int volumeTotalMaxLength = summary.Max(s => s.VolumeTotal.Length);
            int volumeTotalColumnLength = GetColumnLength(volumeTotalHeader, volumeTotalMaxLength);

            const string realizedPnLHeader = "RealizedPnL";
            int realizedPnLMaxLength = summary.Max(s => s.RealizedPnL.Length);
            int realizedPnLColumnLength = GetColumnLength(realizedPnLHeader, realizedPnLMaxLength);

            const string unrealizedPnLHeader = "UnrealizedPnL";
            int unrealizedPnLMaxLength = summary.Max(s => s.UnrealizedPnL.Length);
            int unrealizedPnLColumnLength = GetColumnLength(unrealizedPnLHeader, unrealizedPnLMaxLength);

            const string lykkeBalancesHeader = "Lykke Balances";
            int lykkeBalancesMaxLength = summary.Max(s => s.LykkeBalances.Length);
            int lykkeBalancesColumnLength = GetColumnLength(lykkeBalancesHeader, lykkeBalancesMaxLength);

            const string externalBalancesHeader = "External Balances";
            int externalBalancesMaxLength = summary.Max(s => s.ExternalBalances.Length);
            int externalBalancesColumnLength = GetColumnLength(externalBalancesHeader, externalBalancesMaxLength);

            var sb = new StringBuilder("<pre>");
            string headerRow = GetRow(GetColumnValue(instanceIdHeader, instanceIdColumnLength),
                GetColumnValue(inventoryHeader, inventoryColumnLength),
                GetColumnValue(volumeTotalHeader, volumeTotalColumnLength),
                GetColumnValue(realizedPnLHeader, realizedPnLColumnLength),
                GetColumnValue(unrealizedPnLHeader, unrealizedPnLColumnLength),
                GetColumnValue(lykkeBalancesHeader, lykkeBalancesColumnLength),
                GetColumnValue(externalBalancesHeader, externalBalancesColumnLength));
            string rowSeparator = GetRowSeparator(headerRow.Length);
            sb.AppendLine(rowSeparator);
            sb.AppendLine(headerRow);
            sb.AppendLine(rowSeparator);
            foreach (var instanceSummary in summary)
            {
                string row = GetRow(GetColumnValue(instanceSummary.InstanceId, instanceIdColumnLength),
                    GetColumnValue(instanceSummary.AbsoluteInventory, inventoryColumnLength),
                    GetColumnValue(instanceSummary.VolumeTotal, volumeTotalColumnLength),
                    GetColumnValue(instanceSummary.RealizedPnL, realizedPnLColumnLength),
                    GetColumnValue(instanceSummary.UnrealizedPnL, unrealizedPnLColumnLength),
                    GetColumnValue(instanceSummary.LykkeBalances, lykkeBalancesColumnLength),
                    GetColumnValue(instanceSummary.ExternalBalances, externalBalancesColumnLength));
                sb.AppendLine(row);
                sb.AppendLine(rowSeparator);
            }
            sb.Append("</pre>");
            return sb.ToString();
        }

        private string GetRowSeparator(int length)
        {
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append("—");
            }

            return sb.ToString();
        }

        private string GetRow(string instanceId, string inventory, string volumeTotal, string realizedPnL,
            string unrealizedPnL, string lykkeBalances, string externalBalances)
        {
            return $"| {instanceId} | {inventory} | {volumeTotal} | {realizedPnL} | {unrealizedPnL} | {lykkeBalances} | {externalBalances} |";
        }

        private int GetColumnLength(string header, int maxLength)
        {
            if (header == null)
            {
                header = string.Empty;
            }

            if (maxLength < header.Length)
                return header.Length;
            return maxLength;
        }

        private string GetColumnValue(string value, int length)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            var sb = new StringBuilder(length);
            sb.Append(value);
            for (int i = value.Length; i < length; i++)
            {
                sb.Append(" ");
            }

            return sb.ToString();
        }
    }
}
