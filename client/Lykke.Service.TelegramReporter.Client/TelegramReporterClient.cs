using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.TelegramReporter.Client.AutorestClient;
using Lykke.Service.TelegramReporter.Client.AutorestClient.Models;
using Lykke.Service.TelegramReporter.Client.Exceptions;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Lykke.Service.TelegramReporter.Client
{
    public class TelegramReporterClient : ITelegramReporterClient, IDisposable
    {
        private readonly ILog _log;
        private readonly TelegramReporterAPI _api;

        public TelegramReporterClient(string serviceUrl, ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
            _api = new TelegramReporterAPI(new Uri(serviceUrl));
        }

        public void Dispose()
        {
            _api.Dispose();
        }

        public async Task<IsAliveResponse> IsAliveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.IsAliveWithHttpMessagesAsync(cancellationToken: cancellationToken);
            ValidateResponse(response);
            return JsonConvert.DeserializeObject<IsAliveResponse>(JsonConvert.SerializeObject(response.Body));
        }

        public async Task<IList<ChatPublisherSettingsDto>> GetNeChatPublisherSettingsAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherNechatpublishersettingsGetWithHttpMessagesAsync(cancellationToken: cancellationToken);
            ValidateResponse(response);
            return response.Body;
        }

        public async Task<IList<ChatPublisherSettingsDto>> GetBalanceChatPublisherSettingsAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherBalancechatpublishersettingsGetWithHttpMessagesAsync(cancellationToken: cancellationToken);
            ValidateResponse(response);
            return response.Body;
        }

        public async Task<IList<ChatPublisherSettingsDto>> GetExternalBalanceChatPublisherSettingsAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherExternalbalancechatpublishersettingsGetWithHttpMessagesAsync(cancellationToken: cancellationToken);
            ValidateResponse(response);
            return response.Body;
        }

        public async Task AddNeChatPublisherSettingsAsync(ChatPublisherSettingsPost chatPublisher, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherNechatpublishersettingsPutWithHttpMessagesAsync(chatPublisher, cancellationToken: cancellationToken);
            ValidateResponse(response);
        }

        public async Task AddBalanceChatPublisherSettingsAsync(ChatPublisherSettingsPost chatPublisher, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherBalancechatpublishersettingsPutWithHttpMessagesAsync(chatPublisher, cancellationToken: cancellationToken);
            ValidateResponse(response);
        }

        public async Task AddExternalBalanceChatPublisherSettingsAsync(ChatPublisherSettingsPost chatPublisher, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherExternalbalancechatpublishersettingsPutWithHttpMessagesAsync(chatPublisher, cancellationToken: cancellationToken);
            ValidateResponse(response);
        }

        public async Task RemoveNeChatPublisherSettingsAsync(string chatPublisherSettingsId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherNechatpublishersettingsDeleteWithHttpMessagesAsync(chatPublisherSettingsId, cancellationToken: cancellationToken);
            ValidateResponse(response);
        }

        public async Task RemoveBalanceChatPublisherSettingsAsync(string chatPublisherSettingsId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherBalancechatpublishersettingsDeleteWithHttpMessagesAsync(chatPublisherSettingsId, cancellationToken: cancellationToken);
            ValidateResponse(response);
        }

        public async Task RemoveExternalBalanceChatPublisherSettingsAsync(string chatPublisherSettingsId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherExternalbalancechatpublishersettingsDeleteWithHttpMessagesAsync(chatPublisherSettingsId, cancellationToken: cancellationToken);
            ValidateResponse(response);
        }

        public async Task<IList<BalanceWarningDto>> GetBalancesWarningsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherBalanceswarningsGetWithHttpMessagesAsync(cancellationToken: cancellationToken);
            ValidateResponse(response);
            return response.Body;
        }

        public async Task<IList<ExternalBalanceWarningDto>> GetExternalBalancesWarningsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherExternalbalanceswarningsGetWithHttpMessagesAsync(cancellationToken: cancellationToken);
            ValidateResponse(response);
            return response.Body;
        }

        public async Task AddBalanceWarningAsync(BalanceWarningPost balanceWarning, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherBalancewarningPutWithHttpMessagesAsync(balanceWarning, cancellationToken: cancellationToken);
            ValidateResponse(response);
        }

        public async Task AddExternalBalanceWarningAsync(ExternalBalanceWarningPost balanceWarning, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherExternalbalancewarningPutWithHttpMessagesAsync(balanceWarning, cancellationToken: cancellationToken);
            ValidateResponse(response);
        }

        public async Task RemoveBalanceWarningAsync(string clientId, string assetId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherBalancewarningDeleteWithHttpMessagesAsync(clientId, assetId, cancellationToken: cancellationToken);
            ValidateResponse(response);
        }

        public async Task RemoveExternalBalanceWarningAsync(string exchange, string assetId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherExternalbalancewarningDeleteWithHttpMessagesAsync(exchange, assetId, cancellationToken: cancellationToken);
            ValidateResponse(response);
        }

        private static void ValidateResponse<T>(IHttpOperationResponse<T> response, bool throwIfNotFound = true)
        {
            var error = response.Body as ErrorResponse;
            if (error != null)
            {
                throw new ApiException(error.ErrorMessage);
            }

            if (!response.Response.IsSuccessStatusCode)
            {
                ThrowIfErrorStatus(response.Response.StatusCode, response.Response.ReasonPhrase, throwIfNotFound);
            }
        }

        private static void ValidateResponse(IHttpOperationResponse response)
        {
            if (!response.Response.IsSuccessStatusCode)
            {
                ThrowIfErrorStatus(response.Response.StatusCode, response.Response.ReasonPhrase);
            }
        }

        private static void ThrowIfErrorStatus(HttpStatusCode statusCode, string errorMessage, bool throwIfNotFound = true)
        {
            switch (statusCode)
            {
                case HttpStatusCode.Conflict:
                    throw new ConflictException("Entity with the same key already exists.");
                case HttpStatusCode.NotFound:
                    if (throwIfNotFound)
                    {
                        throw new NotFoundException("Entity is not found.");
                    }
                    break;
                default:
                    throw new ApiException(errorMessage);
            }
        }
    }
}
