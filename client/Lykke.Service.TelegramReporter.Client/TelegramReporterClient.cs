using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
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

        public TelegramReporterClient(string serviceUrl, ILog log)
        {
            _log = log;
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

        public async Task<IList<ChatPublisherSettingsDto>> GetCmlChatPublisherSettings(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherCmlchatpublishersettingsGetWithHttpMessagesAsync(cancellationToken: cancellationToken);
            ValidateResponse(response);
            return response.Body;
        }

        public async Task<IList<ChatPublisherSettingsDto>> GetSeChatPublisherSettings(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherSechatpublishersettingsGetWithHttpMessagesAsync(cancellationToken: cancellationToken);
            ValidateResponse(response);
            return response.Body;
        }

        public async Task AddCmlChatPublisherSettingsAsync(ChatPublisherSettingsPost chatPublisher, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherCmlchatpublishersettingsPutWithHttpMessagesAsync(chatPublisher, cancellationToken: cancellationToken);
            ValidateResponse(response);
        }

        public async Task AddSeChatPublisherSettingsAsync(ChatPublisherSettingsPost chatPublisher, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherSechatpublishersettingsPutWithHttpMessagesAsync(chatPublisher, cancellationToken: cancellationToken);
            ValidateResponse(response);
        }

        public async Task RemoveCmlChatPublisherSettingsAsync(string chatPublisherSettingsId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherCmlchatpublishersettingsDeleteWithHttpMessagesAsync(chatPublisherSettingsId, cancellationToken: cancellationToken);
            ValidateResponse(response);
        }

        public async Task RemoveSeChatPublisherSettingsAsync(string chatPublisherSettingsId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _api.ApiV1ChatPublisherSechatpublishersettingsDeleteWithHttpMessagesAsync(chatPublisherSettingsId, cancellationToken: cancellationToken);
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
