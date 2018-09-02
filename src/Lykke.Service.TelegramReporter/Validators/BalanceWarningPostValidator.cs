using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.TelegramReporter.Client.Models;

namespace Lykke.Service.TelegramReporter.Validators
{
    [UsedImplicitly]
    public class BalanceWarningPostValidator : AbstractValidator<BalanceWarningPost>
    {
        public BalanceWarningPostValidator()
        {
            RuleFor(o => o.ClientId)
                .NotEmpty()
                .WithMessage($"{nameof(BalanceWarningPost.ClientId)} required");

            RuleFor(o => o.AssetId)
                .NotEmpty()
                .WithMessage($"{nameof(BalanceWarningPost.AssetId)} required");

            RuleFor(o => o.Name)
                .NotEmpty()
                .WithMessage($"{nameof(BalanceWarningPost.Name)} required");

            RuleFor(o => o.MinBalance)
                .NotEqual(0)
                .WithMessage($"{nameof(BalanceWarningPost.MinBalance)} required");
        }
    }
}
