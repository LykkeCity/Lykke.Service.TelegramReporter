using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
