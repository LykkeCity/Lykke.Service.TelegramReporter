using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.TelegramReporter.Client.Models;
using Refit;

namespace Lykke.Service.TelegramReporter.Client.Api
{
    [PublicAPI]
    public interface ITelegramReporterApi
    {
        [Get("/api/v1/chatpublishercontroller/nechatpublishersettings")]
        Task<IReadOnlyList<ChatPublisherSettingsDto>> GetNeChatPublisherSettingsAsync();
        [Get("/api/v1/chatpublishercontroller/balancechatpublishersettings")]
        Task<IReadOnlyList<ChatPublisherSettingsDto>> GetBalanceChatPublisherSettingsAsync();
        [Get("/api/v1/chatpublishercontroller/externalbalancechatpublishersettings")]
        Task<IReadOnlyList<ChatPublisherSettingsDto>> GetExternalBalanceChatPublisherSettingsAsync();

        [Post("/api/v1/chatpublishercontroller/nechatpublishersettings")]
        Task AddNeChatPublisherSettingsAsync(ChatPublisherSettingsPost chatPublisher);
        [Post("/api/v1/chatpublishercontroller/balancechatpublishersettings")]
        Task AddBalanceChatPublisherSettingsAsync(ChatPublisherSettingsPost chatPublisher);
        [Post("/api/v1/chatpublishercontroller/externalbalancechatpublishersettings")]
        Task AddExternalBalanceChatPublisherSettingsAsync(ChatPublisherSettingsPost chatPublisher);

        [Delete("/api/v1/chatpublishercontroller/nechatpublishersettings")]
        Task RemoveNeChatPublisherSettingsAsync(string chatPublisherSettingsId);
        [Delete("/api/v1/chatpublishercontroller/balancechatpublishersettings")]
        Task RemoveBalanceChatPublisherSettingsAsync(string chatPublisherSettingsId);
        [Delete("/api/v1/chatpublishercontroller/externalbalancechatpublishersettings")]
        Task RemoveExternalBalanceChatPublisherSettingsAsync(string chatPublisherSettingsId);

        [Get("/api/v1/chatpublishercontroller/balanceswarnings")]
        Task<IReadOnlyList<BalanceWarningDto>> GetBalancesWarningsAsync();
        [Get("/api/v1/chatpublishercontroller/externalbalanceswarnings")]
        Task<IReadOnlyList<ExternalBalanceWarningDto>> GetExternalBalancesWarningsAsync();

        [Post("/api/v1/chatpublishercontroller/balancewarning")]
        Task AddBalanceWarningAsync(BalanceWarningPost balanceWarning);
        [Post("/api/v1/chatpublishercontroller/externalbalancewarning")]
        Task AddExternalBalanceWarningAsync(ExternalBalanceWarningPost balanceWarning);

        [Delete("/api/v1/chatpublishercontroller/balancewarning")]
        Task RemoveBalanceWarningAsync(string clientId, string assetId);
        [Delete("/api/v1/chatpublishercontroller/externalbalancewarning")]
        Task RemoveExternalBalanceWarningAsync(string exchange, string assetId);
    }
}
