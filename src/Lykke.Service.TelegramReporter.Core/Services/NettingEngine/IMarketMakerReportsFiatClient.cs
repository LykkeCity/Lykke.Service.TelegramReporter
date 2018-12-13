using Lykke.Service.MarketMakerReports.Client;

namespace Lykke.Service.TelegramReporter.Core.Services.NettingEngine
{
    public interface IMarketMakerReportsFiatClient
    {
        IMarketMakerReportsClient Client { get; }
    }

    public class MarketMakerReportsFiatClient: IMarketMakerReportsFiatClient
    {
        public MarketMakerReportsFiatClient(IMarketMakerReportsClient client)
        {
            Client = client;
        }

        public IMarketMakerReportsClient Client { get; }
    }
}
