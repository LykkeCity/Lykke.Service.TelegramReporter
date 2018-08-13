
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
        Task<IList<ChatPublisherSettingsDto>> GetCmlChatPublisherSettingsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets SE chat publishers.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>SE chat publishers</returns>
        Task<IList<ChatPublisherSettingsDto>> GetSeChatPublisherSettingsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets NE chat publishers.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>NE chat publishers</returns>
        Task<IList<ChatPublisherSettingsDto>> GetNeChatPublisherSettingsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets Balance chat publishers.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Balance chat publishers</returns>
        Task<IList<ChatPublisherSettingsDto>> GetBalanceChatPublisherSettingsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets External Balance chat publishers.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>External Balance chat publishers</returns>
        Task<IList<ChatPublisherSettingsDto>> GetExternalBalanceChatPublisherSettingsAsync(CancellationToken cancellationToken = default(CancellationToken));

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
        /// Adds NE chat publisher.
        /// </summary>
        /// <param name="chatPublisher">NE chat publisher to add.</param>
        /// <param name="cancellationToken"></param>
        Task AddNeChatPublisherSettingsAsync(ChatPublisherSettingsPost chatPublisher, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds Balance chat publisher.
        /// </summary>
        /// <param name="chatPublisher">Balance chat publisher to add.</param>
        /// <param name="cancellationToken"></param>
        Task AddBalanceChatPublisherSettingsAsync(ChatPublisherSettingsPost chatPublisher, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds External Balance chat publisher.
        /// </summary>
        /// <param name="chatPublisher">External Balance chat publisher to add.</param>
        /// <param name="cancellationToken"></param>
        Task AddExternalBalanceChatPublisherSettingsAsync(ChatPublisherSettingsPost chatPublisher, CancellationToken cancellationToken = default(CancellationToken));

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

        /// <summary>
        /// Deletes NE chat publisher.
        /// </summary>
        /// <param name="chatPublisherSettingsId">NE chat publisher Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveNeChatPublisherSettingsAsync(string chatPublisherSettingsId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes Balance chat publisher.
        /// </summary>
        /// <param name="chatPublisherSettingsId">Balance chat publisher Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveBalanceChatPublisherSettingsAsync(string chatPublisherSettingsId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes External Balance chat publisher.
        /// </summary>
        /// <param name="chatPublisherSettingsId">External Balance chat publisher Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveExternalBalanceChatPublisherSettingsAsync(string chatPublisherSettingsId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets Balances warnings.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Balances warnings</returns>
        Task<IList<BalanceWarningDto>> GetBalancesWarningsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets External Balances warnings.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>External Balances warnings</returns>
        Task<IList<ExternalBalanceWarningDto>> GetExternalBalancesWarningsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds Balance warning.
        /// </summary>
        /// <param name="balanceWarning">Balance warning to add.</param>
        /// <param name="cancellationToken"></param>
        Task AddBalanceWarningAsync(BalanceWarningPost balanceWarning, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds External Balance warning.
        /// </summary>
        /// <param name="balanceWarning">External Balance warning to add.</param>
        /// <param name="cancellationToken"></param>
        Task AddExternalBalanceWarningAsync(ExternalBalanceWarningPost balanceWarning, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes Balance warning.
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <param name="assetId">Asset Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveBalanceWarningAsync(string clientId, string assetId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes External Balance warning.
        /// </summary>
        /// <param name="exchange">Exchange</param>
        /// <param name="assetId">Asset Id</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveExternalBalanceWarningAsync(string exchange, string assetId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
