namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Data
{
    public class ItemViewModel
    {
        public ItemViewModel()
        {
        }

        public ItemViewModel(string id)
        {
            Id = id;
            Title = id;
        }

        public ItemViewModel(string id, bool selected)
            : this(id)
        {
            Selected = selected;
        }

        public ItemViewModel(string id, string title)
        {
            Id = id;
            Title = title;
        }

        public ItemViewModel(string id, string title, bool selected)
            : this(id, title)
        {
            Selected = selected;
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public bool Selected { get; set; }
    }
}
