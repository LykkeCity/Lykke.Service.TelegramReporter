
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Client.AutorestClient.Models;

namespace Lykke.Service.TelegramReporter.Client
{
    public interface ITelegramReporterClient
    {
        /// <summary>
        /// Checks if service is alive
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>IsAlive response</returns>
        /// <exception cref="Exceptions.ApiException">Thrown on getting error response.</exception>
        /// <exception cref="Microsoft.Rest.HttpOperationException">Thrown on getting incorrect http response.</exception>
        /// <exception cref="OperationCanceledException">Thrown on canceled token</exception>
        Task<IsAliveResponse> IsAliveAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets CML chat publishers.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>CML chat publishers</returns>
        Task<IList<ChatPublisherSettingsDto>> GetCmlChatPublisherSettings(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets SE chat publishers.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>SE chat publishers</returns>
        Task<IList<ChatPublisherSettingsDto>> GetSeChatPublisherSettings(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds CML chat publisher.
        /// </summary>
        /// <param name="chatPublisher">CML chat publisher to add.</param>
        /// <param name="cancellationToken"></param>
        Task AddCmlChatPublisherSettingsAsync(ChatPublisherSettingsPost chatPublisher, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds SE chat publisher.
        /// </summary>
        /// <param name="chatPublisher">SE chat publisher to add.</param>
        /// <param name="cancellationToken"></param>
        Task AddSeChatPublisherSettingsAsync(ChatPublisherSettingsPost chatPublisher, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes CML chat publisher.
        /// </summary>
        /// <param name="chatPublisherSettingsId">CML chat publisher Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveCmlChatPublisherSettingsAsync(string chatPublisherSettingsId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes SE chat publisher.
        /// </summary>
        /// <param name="chatPublisherSettingsId">SE chat publisher Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveSeChatPublisherSettingsAsync(string chatPublisherSettingsId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
