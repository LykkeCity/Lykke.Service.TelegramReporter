using AutoMapper;
using Lykke.Service.TelegramReporter.Client.Models;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.AutoMapperProfiles
{
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<IChatPublisherSettings, ChatPublisherSettingsDto>();
            CreateMap<ChatPublisherSettingsPost, ChatPublisherSettings>()
                .ForMember(x => x.ChatPublisherSettingsId, opt => opt.Ignore());

            CreateMap<IBalanceWarning, BalanceWarningDto>();
            CreateMap<BalanceWarningPost, BalanceWarning>();

            CreateMap<IExternalBalanceWarning, ExternalBalanceWarningDto>();
            CreateMap<ExternalBalanceWarningPost, ExternalBalanceWarning>();
        }
    }
}
