using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.TelegramReporter.Client.Models;

namespace Lykke.Service.TelegramReporter.Validators
{
    [UsedImplicitly]
    public class ChatPublisherSettingsPostValidator : AbstractValidator<ChatPublisherSettingsPost>
    {
        public ChatPublisherSettingsPostValidator()
        {
            RuleFor(o => o.ChatId)
                .NotEqual(0)
                .WithMessage($"{nameof(ChatPublisherSettingsPost.ChatId)} required");

            RuleFor(o => o.TimeSpan.Ticks)
                .NotEqual(0)
                .WithMessage($"{nameof(ChatPublisherSettingsPost.TimeSpan)} required");
        }
    }
}
