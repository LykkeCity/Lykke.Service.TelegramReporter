using AutoMapper;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Models;

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
        }
    }
}
