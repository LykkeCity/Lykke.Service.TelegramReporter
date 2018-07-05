using System;
using System.Runtime.Serialization;

namespace Lykke.Service.TelegramReporter.Client.Exceptions
{
    [Serializable]
    public class ApiException : Exception
    {
        public ApiException()
        {
        }

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ApiException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
