using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.Dwh.Client;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Channelv2;

namespace Lykke.Service.TelegramReporter.Services.Channelv2.Channels
{
    public class DwhStoreProcedureChannel : ReportChannel
    {
        private readonly IDwhClient _dwhClient;
        public const string Name = "DwhStoreProcedure";

        public DwhStoreProcedureChannel(IReportChannel channel, ITelegramSender telegramSender, ILogFactory logFactory,
            IDwhClient dwhClient) : base(channel, telegramSender, logFactory)
        {
            _dwhClient = dwhClient;
        }

        protected override async Task DoTimer()
        {
            ResponceDataSet response = await ExecuteProcedure();

            var report = response.Data.Tables
                .Cast<DataTable>()
                .ToArray();


            foreach (var table in report)
            {
                foreach (DataRow row in table.Rows)
                {
                    var str = new StringBuilder();
                    foreach (DataColumn column in table.Columns)
                    {
                        str.AppendLine($"{column.ColumnName}: {row[column.ColumnName]}");
                    }

                    await SendMessage(str.ToString());
                }
            }
        }

        private async Task<ResponceDataSet> ExecuteProcedure()
        {
            int countTry = 3;
            while (countTry > 0)
            {
                countTry--;
                try
                {
                    ResponceDataSet response =
                        await _dwhClient.Call(new Dictionary<string, string>(), Metainfo, "report");
                    return response;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, context: $"Procedure: {Metainfo}");
                    if (countTry <= 0)
                    {
                        throw;
                    }
                }

                await Task.Delay(5000);
            }

            throw new Exception("Cannot execute store procedure");
        }
    }
}
