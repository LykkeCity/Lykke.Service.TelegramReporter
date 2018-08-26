using JetBrains.Annotations;
using Lykke.Service.TelegramReporter.Client.Api;

namespace Lykke.Service.TelegramReporter.Client
{
    /// <summary>
    /// TelegramReporter client interface.
    /// </summary>
    [PublicAPI]
    public interface ITelegramReporterClient
    {
        /// <summary>Api for TelegramReporter</summary>
        ITelegramReporterApi TelegramReporterApi { get; }
    }
}
