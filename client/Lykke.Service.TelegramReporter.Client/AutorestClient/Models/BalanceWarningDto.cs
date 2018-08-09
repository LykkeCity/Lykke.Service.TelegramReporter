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

    public partial class BalanceWarningDto
    {
        /// <summary>
        /// Initializes a new instance of the BalanceWarningDto class.
        /// </summary>
        public BalanceWarningDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the BalanceWarningDto class.
        /// </summary>
        public BalanceWarningDto(string clientId, string assetId, string name, double minBalance)
        {
            ClientId = clientId;
            AssetId = assetId;
            Name = name;
            MinBalance = minBalance;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ClientId")]
        public string ClientId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "AssetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "MinBalance")]
        public double MinBalance { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (ClientId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ClientId");
            }
            if (AssetId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "AssetId");
            }
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
        }
    }
}
