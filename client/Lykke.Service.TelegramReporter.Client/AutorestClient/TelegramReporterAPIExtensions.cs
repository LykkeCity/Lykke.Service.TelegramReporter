// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.TelegramReporter.Client.AutorestClient
{
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for TelegramReporterAPI.
    /// </summary>
    public static partial class TelegramReporterAPIExtensions
    {
            /// <summary>
            /// Gets CML chat publishers.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IList<ChatPublisherSettingsDto> ApiV1ChatPublisherCmlchatpublishersettingsGet(this ITelegramReporterAPI operations)
            {
                return operations.ApiV1ChatPublisherCmlchatpublishersettingsGetAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets CML chat publishers.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<ChatPublisherSettingsDto>> ApiV1ChatPublisherCmlchatpublishersettingsGetAsync(this ITelegramReporterAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherCmlchatpublishersettingsGetWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Adds CML chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisher'>
            /// CML chat publisher to add.
            /// </param>
            public static ErrorResponse ApiV1ChatPublisherCmlchatpublishersettingsPut(this ITelegramReporterAPI operations, ChatPublisherSettingsPost chatPublisher = default(ChatPublisherSettingsPost))
            {
                return operations.ApiV1ChatPublisherCmlchatpublishersettingsPutAsync(chatPublisher).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Adds CML chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisher'>
            /// CML chat publisher to add.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ErrorResponse> ApiV1ChatPublisherCmlchatpublishersettingsPutAsync(this ITelegramReporterAPI operations, ChatPublisherSettingsPost chatPublisher = default(ChatPublisherSettingsPost), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherCmlchatpublishersettingsPutWithHttpMessagesAsync(chatPublisher, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Deletes CML chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisherSettingsId'>
            /// CML chat publisher settings Id
            /// </param>
            public static void ApiV1ChatPublisherCmlchatpublishersettingsDelete(this ITelegramReporterAPI operations, string chatPublisherSettingsId = default(string))
            {
                operations.ApiV1ChatPublisherCmlchatpublishersettingsDeleteAsync(chatPublisherSettingsId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Deletes CML chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisherSettingsId'>
            /// CML chat publisher settings Id
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task ApiV1ChatPublisherCmlchatpublishersettingsDeleteAsync(this ITelegramReporterAPI operations, string chatPublisherSettingsId = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.ApiV1ChatPublisherCmlchatpublishersettingsDeleteWithHttpMessagesAsync(chatPublisherSettingsId, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Gets SE chat publishers.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IList<ChatPublisherSettingsDto> ApiV1ChatPublisherSechatpublishersettingsGet(this ITelegramReporterAPI operations)
            {
                return operations.ApiV1ChatPublisherSechatpublishersettingsGetAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets SE chat publishers.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<ChatPublisherSettingsDto>> ApiV1ChatPublisherSechatpublishersettingsGetAsync(this ITelegramReporterAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherSechatpublishersettingsGetWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Adds SE chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisher'>
            /// SE chat publisher to add.
            /// </param>
            public static ErrorResponse ApiV1ChatPublisherSechatpublishersettingsPut(this ITelegramReporterAPI operations, ChatPublisherSettingsPost chatPublisher = default(ChatPublisherSettingsPost))
            {
                return operations.ApiV1ChatPublisherSechatpublishersettingsPutAsync(chatPublisher).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Adds SE chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisher'>
            /// SE chat publisher to add.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ErrorResponse> ApiV1ChatPublisherSechatpublishersettingsPutAsync(this ITelegramReporterAPI operations, ChatPublisherSettingsPost chatPublisher = default(ChatPublisherSettingsPost), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherSechatpublishersettingsPutWithHttpMessagesAsync(chatPublisher, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Deletes SE chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisherSettingsId'>
            /// SE chat publisher settings Id
            /// </param>
            public static void ApiV1ChatPublisherSechatpublishersettingsDelete(this ITelegramReporterAPI operations, string chatPublisherSettingsId = default(string))
            {
                operations.ApiV1ChatPublisherSechatpublishersettingsDeleteAsync(chatPublisherSettingsId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Deletes SE chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisherSettingsId'>
            /// SE chat publisher settings Id
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task ApiV1ChatPublisherSechatpublishersettingsDeleteAsync(this ITelegramReporterAPI operations, string chatPublisherSettingsId = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.ApiV1ChatPublisherSechatpublishersettingsDeleteWithHttpMessagesAsync(chatPublisherSettingsId, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Gets NE chat publishers.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IList<ChatPublisherSettingsDto> ApiV1ChatPublisherNechatpublishersettingsGet(this ITelegramReporterAPI operations)
            {
                return operations.ApiV1ChatPublisherNechatpublishersettingsGetAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets NE chat publishers.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<ChatPublisherSettingsDto>> ApiV1ChatPublisherNechatpublishersettingsGetAsync(this ITelegramReporterAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherNechatpublishersettingsGetWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Adds NE chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisher'>
            /// NE chat publisher to add.
            /// </param>
            public static ErrorResponse ApiV1ChatPublisherNechatpublishersettingsPut(this ITelegramReporterAPI operations, ChatPublisherSettingsPost chatPublisher = default(ChatPublisherSettingsPost))
            {
                return operations.ApiV1ChatPublisherNechatpublishersettingsPutAsync(chatPublisher).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Adds NE chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisher'>
            /// NE chat publisher to add.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ErrorResponse> ApiV1ChatPublisherNechatpublishersettingsPutAsync(this ITelegramReporterAPI operations, ChatPublisherSettingsPost chatPublisher = default(ChatPublisherSettingsPost), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherNechatpublishersettingsPutWithHttpMessagesAsync(chatPublisher, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Deletes NE chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisherSettingsId'>
            /// NE chat publisher settings Id
            /// </param>
            public static void ApiV1ChatPublisherNechatpublishersettingsDelete(this ITelegramReporterAPI operations, string chatPublisherSettingsId = default(string))
            {
                operations.ApiV1ChatPublisherNechatpublishersettingsDeleteAsync(chatPublisherSettingsId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Deletes NE chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisherSettingsId'>
            /// NE chat publisher settings Id
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task ApiV1ChatPublisherNechatpublishersettingsDeleteAsync(this ITelegramReporterAPI operations, string chatPublisherSettingsId = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.ApiV1ChatPublisherNechatpublishersettingsDeleteWithHttpMessagesAsync(chatPublisherSettingsId, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Gets balance chat publishers.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IList<ChatPublisherSettingsDto> ApiV1ChatPublisherBalancechatpublishersettingsGet(this ITelegramReporterAPI operations)
            {
                return operations.ApiV1ChatPublisherBalancechatpublishersettingsGetAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets balance chat publishers.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<ChatPublisherSettingsDto>> ApiV1ChatPublisherBalancechatpublishersettingsGetAsync(this ITelegramReporterAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherBalancechatpublishersettingsGetWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Adds balance chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisher'>
            /// Balance chat publisher to add.
            /// </param>
            public static ErrorResponse ApiV1ChatPublisherBalancechatpublishersettingsPut(this ITelegramReporterAPI operations, ChatPublisherSettingsPost chatPublisher = default(ChatPublisherSettingsPost))
            {
                return operations.ApiV1ChatPublisherBalancechatpublishersettingsPutAsync(chatPublisher).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Adds balance chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisher'>
            /// Balance chat publisher to add.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ErrorResponse> ApiV1ChatPublisherBalancechatpublishersettingsPutAsync(this ITelegramReporterAPI operations, ChatPublisherSettingsPost chatPublisher = default(ChatPublisherSettingsPost), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherBalancechatpublishersettingsPutWithHttpMessagesAsync(chatPublisher, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Deletes balance chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisherSettingsId'>
            /// Balance chat publisher settings Id
            /// </param>
            public static void ApiV1ChatPublisherBalancechatpublishersettingsDelete(this ITelegramReporterAPI operations, string chatPublisherSettingsId = default(string))
            {
                operations.ApiV1ChatPublisherBalancechatpublishersettingsDeleteAsync(chatPublisherSettingsId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Deletes balance chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisherSettingsId'>
            /// Balance chat publisher settings Id
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task ApiV1ChatPublisherBalancechatpublishersettingsDeleteAsync(this ITelegramReporterAPI operations, string chatPublisherSettingsId = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.ApiV1ChatPublisherBalancechatpublishersettingsDeleteWithHttpMessagesAsync(chatPublisherSettingsId, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Gets external balance chat publishers.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IList<ChatPublisherSettingsDto> ApiV1ChatPublisherExternalbalancechatpublishersettingsGet(this ITelegramReporterAPI operations)
            {
                return operations.ApiV1ChatPublisherExternalbalancechatpublishersettingsGetAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets external balance chat publishers.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<ChatPublisherSettingsDto>> ApiV1ChatPublisherExternalbalancechatpublishersettingsGetAsync(this ITelegramReporterAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherExternalbalancechatpublishersettingsGetWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Adds external balance chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisher'>
            /// External balance chat publisher to add.
            /// </param>
            public static ErrorResponse ApiV1ChatPublisherExternalbalancechatpublishersettingsPut(this ITelegramReporterAPI operations, ChatPublisherSettingsPost chatPublisher = default(ChatPublisherSettingsPost))
            {
                return operations.ApiV1ChatPublisherExternalbalancechatpublishersettingsPutAsync(chatPublisher).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Adds external balance chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisher'>
            /// External balance chat publisher to add.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ErrorResponse> ApiV1ChatPublisherExternalbalancechatpublishersettingsPutAsync(this ITelegramReporterAPI operations, ChatPublisherSettingsPost chatPublisher = default(ChatPublisherSettingsPost), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherExternalbalancechatpublishersettingsPutWithHttpMessagesAsync(chatPublisher, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Deletes external balance chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisherSettingsId'>
            /// External balance chat publisher settings Id
            /// </param>
            public static void ApiV1ChatPublisherExternalbalancechatpublishersettingsDelete(this ITelegramReporterAPI operations, string chatPublisherSettingsId = default(string))
            {
                operations.ApiV1ChatPublisherExternalbalancechatpublishersettingsDeleteAsync(chatPublisherSettingsId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Deletes external balance chat publisher.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='chatPublisherSettingsId'>
            /// External balance chat publisher settings Id
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task ApiV1ChatPublisherExternalbalancechatpublishersettingsDeleteAsync(this ITelegramReporterAPI operations, string chatPublisherSettingsId = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.ApiV1ChatPublisherExternalbalancechatpublishersettingsDeleteWithHttpMessagesAsync(chatPublisherSettingsId, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Gets balances warnings.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IList<BalanceWarningDto> ApiV1ChatPublisherBalanceswarningsGet(this ITelegramReporterAPI operations)
            {
                return operations.ApiV1ChatPublisherBalanceswarningsGetAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets balances warnings.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<BalanceWarningDto>> ApiV1ChatPublisherBalanceswarningsGetAsync(this ITelegramReporterAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherBalanceswarningsGetWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Gets external balances warnings.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IList<ExternalBalanceWarningDto> ApiV1ChatPublisherExternalbalanceswarningsGet(this ITelegramReporterAPI operations)
            {
                return operations.ApiV1ChatPublisherExternalbalanceswarningsGetAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets external balances warnings.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<ExternalBalanceWarningDto>> ApiV1ChatPublisherExternalbalanceswarningsGetAsync(this ITelegramReporterAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherExternalbalanceswarningsGetWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Adds balance warning.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='balanceWarning'>
            /// Balance warning to add.
            /// </param>
            public static ErrorResponse ApiV1ChatPublisherBalancewarningPut(this ITelegramReporterAPI operations, BalanceWarningPost balanceWarning = default(BalanceWarningPost))
            {
                return operations.ApiV1ChatPublisherBalancewarningPutAsync(balanceWarning).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Adds balance warning.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='balanceWarning'>
            /// Balance warning to add.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ErrorResponse> ApiV1ChatPublisherBalancewarningPutAsync(this ITelegramReporterAPI operations, BalanceWarningPost balanceWarning = default(BalanceWarningPost), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherBalancewarningPutWithHttpMessagesAsync(balanceWarning, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Deletes balance warning.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='clientId'>
            /// Wallet
            /// </param>
            /// <param name='assetId'>
            /// Asset ID
            /// </param>
            public static void ApiV1ChatPublisherBalancewarningDelete(this ITelegramReporterAPI operations, string clientId = default(string), string assetId = default(string))
            {
                operations.ApiV1ChatPublisherBalancewarningDeleteAsync(clientId, assetId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Deletes balance warning.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='clientId'>
            /// Wallet
            /// </param>
            /// <param name='assetId'>
            /// Asset ID
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task ApiV1ChatPublisherBalancewarningDeleteAsync(this ITelegramReporterAPI operations, string clientId = default(string), string assetId = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.ApiV1ChatPublisherBalancewarningDeleteWithHttpMessagesAsync(clientId, assetId, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Adds external balance warning.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='balanceWarning'>
            /// External balance warning to add.
            /// </param>
            public static ErrorResponse ApiV1ChatPublisherExternalbalancewarningPut(this ITelegramReporterAPI operations, ExternalBalanceWarningPost balanceWarning = default(ExternalBalanceWarningPost))
            {
                return operations.ApiV1ChatPublisherExternalbalancewarningPutAsync(balanceWarning).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Adds external balance warning.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='balanceWarning'>
            /// External balance warning to add.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ErrorResponse> ApiV1ChatPublisherExternalbalancewarningPutAsync(this ITelegramReporterAPI operations, ExternalBalanceWarningPost balanceWarning = default(ExternalBalanceWarningPost), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiV1ChatPublisherExternalbalancewarningPutWithHttpMessagesAsync(balanceWarning, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Deletes external balance warning.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='exchange'>
            /// Exchange
            /// </param>
            /// <param name='assetId'>
            /// Asset ID
            /// </param>
            public static void ApiV1ChatPublisherExternalbalancewarningDelete(this ITelegramReporterAPI operations, string exchange = default(string), string assetId = default(string))
            {
                operations.ApiV1ChatPublisherExternalbalancewarningDeleteAsync(exchange, assetId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Deletes external balance warning.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='exchange'>
            /// Exchange
            /// </param>
            /// <param name='assetId'>
            /// Asset ID
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task ApiV1ChatPublisherExternalbalancewarningDeleteAsync(this ITelegramReporterAPI operations, string exchange = default(string), string assetId = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.ApiV1ChatPublisherExternalbalancewarningDeleteWithHttpMessagesAsync(exchange, assetId, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IsAliveResponse IsAlive(this ITelegramReporterAPI operations)
            {
                return operations.IsAliveAsync().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IsAliveResponse> IsAliveAsync(this ITelegramReporterAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.IsAliveWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
