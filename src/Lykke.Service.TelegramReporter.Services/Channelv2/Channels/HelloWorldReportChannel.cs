using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Channelv2;

namespace Lykke.Service.TelegramReporter.Services.Channelv2.Channels
{
    public class HelloWorldReportChannel : ReportChannel
    {
        public const string Name = "helloworld";

        public HelloWorldReportChannel(IReportChannel channel, ITelegramSender telegramSender, ILogFactory logFactory) : base(channel, telegramSender, logFactory)
        {
        }

        protected override Task DoTimer()
        {
            return SendMessage($"Hello world! {Metainfo}");
        }
    }
}
