using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Core.Services.Balance
{
    public interface IBalanceWarningProvider
    {
        Task<string> GetWarningMessageAsync();

        Task<string> GetWarningMessageAsync(string instanceId);
    }
}
