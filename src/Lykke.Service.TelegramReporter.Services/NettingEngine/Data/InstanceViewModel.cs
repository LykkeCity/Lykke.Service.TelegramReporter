namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Data
{
    public class InstanceViewModel
    {
        public InstanceViewModel()
        {
        }

        public InstanceViewModel(int serviceInstanceIndex)
        {
            ServiceInstanceIndex = serviceInstanceIndex;
        }

        public int ServiceInstanceIndex { get; set; }
    }
}
