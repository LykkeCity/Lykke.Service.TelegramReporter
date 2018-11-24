using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.Core.Services.LiquidityEngine
{
    public interface ILiquidityEngineSummaryWarningProvider
    {
        Task<string> GetWarningMessageAsync(ILiquidityEngineSummaryWarning warning);
    }
}
