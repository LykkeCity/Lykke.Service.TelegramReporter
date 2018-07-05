using System;
using System.Runtime.Serialization;

namespace Lykke.Service.TelegramReporter.Client.Exceptions
{
    [Serializable]
    public class ConflictException : ApiException
    {
        public ConflictException()
        {
        }

        public ConflictException(string message) : base(message)
        {
        }

        public ConflictException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ConflictException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
