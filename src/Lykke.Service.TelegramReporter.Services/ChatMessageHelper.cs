namespace Lykke.Service.TelegramReporter.Services
{
    public abstract class ChatMessageHelper
    {
        private const int CharactersLimit = 4096;

        public static string CheckSizeAndCutMessageIfNeeded(string message)
        {
            if (message.Length > CharactersLimit)
            {
                message = message.Substring(0, CharactersLimit);
            }

            return message;
        }
    }
}
