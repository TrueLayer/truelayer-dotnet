using System.Text.Json.Serialization;
using TrueLayer.Serialization;

namespace TrueLayer.Common
{
    /// <summary>
    /// Represents the ultimate counterparty details for sub-merchants
    /// </summary>
    public abstract record UltimateCounterparty
    {
        /// <summary>
        /// Gets the type of the ultimate counterparty
        /// </summary>
        [JsonPropertyName("type")]
        public abstract string Type { get; }
    }

    /// <summary>
    /// Represents business client details for the ultimate counterparty
    /// </summary>
    [JsonDiscriminator("business_client")]
    public record UltimateCounterpartyBusinessClient : UltimateCounterparty
    {
        /// <summary>
        /// Creates a new <see cref="UltimateCounterpartyBusinessClient"/>
        /// </summary>
        /// <param name="commercialName">The commercial name of the business</param>
        /// <param name="mcc">The merchant category code of the business</param>
        /// <param name="address">The address of the business (required if registration_number is not provided)</param>
        /// <param name="registrationNumber">The registration number of the business (required if address is not provided)</param>
        public UltimateCounterpartyBusinessClient(
            string commercialName,
            string? mcc = null,
            Address? address = null,
            string? registrationNumber = null)
        {
            CommercialName = commercialName.NotNullOrWhiteSpace(nameof(commercialName));
            Mcc = mcc;
            Address = address;
            RegistrationNumber = registrationNumber;
        }

        /// <inheritdoc />
        public override string Type => "business_client";

        /// <summary>
        /// Gets the commercial name of the business
        /// </summary>
        [JsonPropertyName("commercial_name")]
        public string CommercialName { get; }

        /// <summary>
        /// Gets the merchant category code of the business
        /// </summary>
        [JsonPropertyName("mcc")]
        public string? Mcc { get; }

        /// <summary>
        /// Gets the address of the business
        /// </summary>
        [JsonPropertyName("address")]
        public Address? Address { get; }

        /// <summary>
        /// Gets the registration number of the business
        /// </summary>
        [JsonPropertyName("registration_number")]
        public string? RegistrationNumber { get; }
    }

    /// <summary>
    /// Represents business division details for the ultimate counterparty
    /// </summary>
    [JsonDiscriminator("business_division")]
    public record UltimateCounterpartyBusinessDivision : UltimateCounterparty
    {
        /// <summary>
        /// Creates a new <see cref="UltimateCounterpartyBusinessDivision"/>
        /// </summary>
        /// <param name="id">The identifier of the business division</param>
        /// <param name="name">The name of the business division</param>
        public UltimateCounterpartyBusinessDivision(string id, string name)
        {
            Id = id.NotNullOrWhiteSpace(nameof(id));
            Name = name.NotNullOrWhiteSpace(nameof(name));
        }

        /// <inheritdoc />
        public override string Type => "business_division";

        /// <summary>
        /// Gets the identifier of the business division
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }

        /// <summary>
        /// Gets the name of the business division
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }
    }
}