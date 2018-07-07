// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.TelegramReporter.Client.AutorestClient.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class ChatPublisherSettingsDto
    {
        /// <summary>
        /// Initializes a new instance of the ChatPublisherSettingsDto class.
        /// </summary>
        public ChatPublisherSettingsDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ChatPublisherSettingsDto class.
        /// </summary>
        public ChatPublisherSettingsDto(string timeSpan, long chatId, string chatPublisherSettingsId = default(string))
        {
            ChatPublisherSettingsId = chatPublisherSettingsId;
            TimeSpan = timeSpan;
            ChatId = chatId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ChatPublisherSettingsId")]
        public string ChatPublisherSettingsId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "TimeSpan")]
        public string TimeSpan { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ChatId")]
        public long ChatId { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (TimeSpan == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "TimeSpan");
            }
        }
    }
}