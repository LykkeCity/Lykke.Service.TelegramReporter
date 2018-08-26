using Lykke.HttpClientGenerator;
using Lykke.Service.TelegramReporter.Client.Api;

namespace Lykke.Service.TelegramReporter.Client
{
    /// <summary>
    /// TelegramReporter API aggregating interface.
    /// </summary>
    public class TelegramReporterClient : ITelegramReporterClient
    {
        /// <summary>Api for TelegramReporter</summary>
        public ITelegramReporterApi TelegramReporterApi { get; private set; }

        /// <summary>C-tor</summary>
        public TelegramReporterClient(IHttpClientGenerator httpClientGenerator)
        {
            TelegramReporterApi = httpClientGenerator.Generate<ITelegramReporterApi>();
        }
    }
}
