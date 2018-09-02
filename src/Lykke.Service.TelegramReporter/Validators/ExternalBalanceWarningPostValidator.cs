using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.TelegramReporter.Client.Models;

namespace Lykke.Service.TelegramReporter.Validators
{
    [UsedImplicitly]
    public class ExternalBalanceWarningPostValidator : AbstractValidator<ExternalBalanceWarningPost>
    {
        public ExternalBalanceWarningPostValidator()
        {
            RuleFor(o => o.Exchange)
                .NotEmpty()
                .WithMessage($"{nameof(ExternalBalanceWarningPost.Exchange)} required");

            RuleFor(o => o.AssetId)
                .NotEmpty()
                .WithMessage($"{nameof(ExternalBalanceWarningPost.AssetId)} required");

            RuleFor(o => o.Name)
                .NotEmpty()
                .WithMessage($"{nameof(ExternalBalanceWarningPost.Name)} required");

            RuleFor(o => o.MinBalance)
                .NotEqual(0)
                .WithMessage($"{nameof(ExternalBalanceWarningPost.MinBalance)} required");
        }
    }
}
