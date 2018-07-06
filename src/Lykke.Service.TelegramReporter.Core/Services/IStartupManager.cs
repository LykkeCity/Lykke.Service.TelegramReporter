using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}
