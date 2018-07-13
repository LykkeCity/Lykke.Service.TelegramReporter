namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Data
{
    public class QuoteViewModel
    {
        public QuoteViewModel()
        {
        }

        public QuoteViewModel(decimal ask, decimal bid)
        {
            Ask = ask;
            Bid = bid;
        }

        public decimal Ask { get; set; }

        public decimal Bid { get; set; }

        public decimal Spread
            => Ask > Bid && Ask > 0 && Bid > 0 ? Ask - Bid : 0;

        public decimal Mid
            => Spread > 0 ? (Ask + Bid) / 2 : 0;

        public decimal SpreadRatio
            => Mid > 0 ? Spread / Mid : 0;
    }
}
