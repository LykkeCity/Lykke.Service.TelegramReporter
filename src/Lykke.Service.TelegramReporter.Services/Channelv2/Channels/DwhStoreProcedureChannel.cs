using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
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
        private readonly ILog _log;

        public DwhStoreProcedureChannel(IReportChannel channel, ITelegramSender telegramSender, ILogFactory logFactory,
            IDwhClient dwhClient, ILog log) : base(channel, telegramSender, logFactory)
        {
            _dwhClient = dwhClient;
            _log = log;
        }

        protected override async Task DoTimer()
        {
            _log.Info("DwhStoreProcedureChannel.DoTimer() - started...");

            ResponceDataSet response = await ExecuteProcedure();

            _log.Info("DwhStoreProcedureChannel.DoTimer() - executed procedure.", response);

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

                    _log.Info("DwhStoreProcedureChannel.DoTimer() - sending a message...", str);

                    await SendMessage(str.ToString());

                    _log.Info("DwhStoreProcedureChannel.DoTimer() - finished sending a message.", str);
                }
            }

            _log.Info("DwhStoreProcedureChannel.DoTimer() - finished.");
        }

        private async Task<ResponceDataSet> ExecuteProcedure()
        {
            int countTry = 3;
            while (countTry > 0)
            {
                countTry--;
                try
                {
                    _log.Info("DwhStoreProcedureChannel.ExecuteProcedure() - calling DWH client...", Metainfo);

                    ResponceDataSet response =
                        await _dwhClient.Call(new Dictionary<string, string>(), Metainfo, "report");

                    _log.Info("DwhStoreProcedureChannel.ExecuteProcedure() - successfully received response.", new
                    {
                        Procedure = Metainfo,
                        Response = response
                    });

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
