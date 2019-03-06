using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.IndexHedgingEngine.Client;
using Lykke.Service.IndexHedgingEngine.Client.Models.Reports;
using Lykke.Service.IndexHedgingEngine.Client.Models.Settlements;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Channelv2;

namespace Lykke.Service.TelegramReporter.Services.Channelv2.Channels
{
    public class IndexHedgingEngineHealthIssuesChannel : ReportChannel
    {
        private readonly IIndexHedgingEngineClient _indexHedgingEngineClient;

        public IndexHedgingEngineHealthIssuesChannel(
            IReportChannel channel,
            ITelegramSender telegramSender,
            ILogFactory logFactory,
            IIndexHedgingEngineClient indexHedgingEngineClient)
            : base(channel, telegramSender, logFactory)
        {
            _indexHedgingEngineClient = indexHedgingEngineClient;
        }

        public static string Name => "IndexHedgingEngineHealthIssues";

        protected override async Task DoTimer()
        {
            await SendPositionErrorMessageAsync();

            await SendActiveSettlementMessageAsync();
        }

        private async Task SendPositionErrorMessageAsync()
        {
            IReadOnlyCollection<PositionReportModel> positionReport =
                await _indexHedgingEngineClient.Reports.GetPositionReportsAsync();

            List<PositionReportModel> positionsWithErrors = positionReport
                .Where(o => !string.IsNullOrWhiteSpace(o.Error))
                .ToList();

            if (positionsWithErrors.Count == 0)
                return;

            var header = "IHE Assets Errors";

            var message = new StringBuilder();

            message.AppendLine(header);

            foreach (PositionReportModel position in positionsWithErrors)
                if (!(position.Error == "No quote" && position.AssetInvestment.RemainingAmount == 0))
                    message.AppendLine($"{position.AssetId}: {position.Error}");

            if (message.ToString() == header)
                return;

            await SendMessage(message.ToString());
        }

        private async Task SendActiveSettlementMessageAsync()
        {
            IReadOnlyCollection<SettlementModel> settlements = await _indexHedgingEngineClient.Settlements.GetAsync();

            List<SettlementModel> activeSettlements = settlements
                .Where(o => o.Status != SettlementStatus.Completed && o.Status != SettlementStatus.Rejected)
                .ToList();

            if (activeSettlements.Count == 0)
                return;

            var message = new StringBuilder();
            message.AppendLine("IHE Active Settlements");

            foreach (SettlementModel settlement in activeSettlements)
            {
                decimal amountInUsd = settlement.Amount * settlement.Price;

                message.AppendLine(
                    $"{settlement.IndexName} {settlement.Amount} (${amountInUsd:N2}) {settlement.Status}");
            }

            await SendMessage(message.ToString());
        }
    }
}
