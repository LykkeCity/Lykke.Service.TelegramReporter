using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Lykke.Common.Log;
using Lykke.Service.Dwh.Client;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Channelv2;

namespace Lykke.Service.TelegramReporter.Services.Channelv2.Channels
{
    public class LyciSandipOfferChannel : ReportChannel
    {
        private readonly IDwhClient _dwhClient;
        public const string Name = "LyciSandipOffer";

        public LyciSandipOfferChannel(IReportChannel channel, ITelegramSender telegramSender, ILogFactory logFactory, IDwhClient dwhClient) 
            : base(channel, telegramSender, logFactory)
        {
            _dwhClient = dwhClient;
        }

        protected override async Task DoTimer()
        {
            ResponceDataSet response = await _dwhClient.Call(new Dictionary<string, string>(), Metainfo, "report");

            var report = response.Data.Tables
                .Cast<DataTable>()
                .ToArray();


            var table = report.FirstOrDefault();
            if (table == null || table.Rows.Count <= 0)
                return;
            
            var str = new StringBuilder();
            str.AppendLine($"please do these actions at cryptofacilities.com");
            str.AppendLine();
            foreach (DataRow row in table.Rows)
            {
                if (row[table.Columns[0].ColumnName].ToString() == "Adjustment_Required_($)")
                {
                    var value = decimal.Parse(row[table.Columns[2].ColumnName].ToString());
                    str.AppendLine($"* Position Adjustment Required at {Math.Round(value, 2)} $, asset: {row[table.Columns[1].ColumnName]}. ");
                }

                if (row[table.Columns[0].ColumnName].ToString() == "Margin_Adjustment_Required_($)")
                {
                    var value = decimal.Parse(row[table.Columns[2].ColumnName].ToString());
                    str.AppendLine($"* Margin Adjustment Required at {Math.Round(value, 2)}, asset: {row[table.Columns[1].ColumnName]}. ");
                }
            }
            await SendMessage(str.ToString());
        }
    }
}
