using System;
using Common.Log;

namespace Lykke.Service.TelegramReporter.Client
{
    public class TelegramReporterClient : ITelegramReporterClient, IDisposable
    {
        private readonly ILog _log;

        public TelegramReporterClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
