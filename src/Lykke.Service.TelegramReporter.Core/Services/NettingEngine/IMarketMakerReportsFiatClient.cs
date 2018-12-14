using Lykke.Service.MarketMakerReports.Client;

namespace Lykke.Service.TelegramReporter.Core.Services.NettingEngine
{
    public interface IMarketMakerReportsFiatClient
    {
        IMarketMakerReportsClient Client { get; }
        string ApiUrl { get; }
    }

    public class MarketMakerReportsFiatClient: IMarketMakerReportsFiatClient
    {
        public MarketMakerReportsFiatClient(IMarketMakerReportsClient client, string apiUrl)
        {
            Client = client;
            ApiUrl = apiUrl;
        }

        public IMarketMakerReportsClient Client { get; }
        public string ApiUrl { get; }
    }
}
