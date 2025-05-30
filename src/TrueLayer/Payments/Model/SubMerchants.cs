using System;
using System.Linq;
using OneOf;
using TrueLayer.Common;
using TrueLayer.Serialization;
using static TrueLayer.Payments.Model.UltimateCounterparty;

namespace TrueLayer.Payments.Model
{
    using UltimateCounterpartyUnion = OneOf<BusinessDivision, BusinessClient>;

    /// <summary>
    /// Represents sub-merchant information for marketplace and platform payments
    /// </summary>
    public record SubMerchants
    {
        /// <summary>
        /// Creates a new <see cref="SubMerchants"/> instance
        /// </summary>
        /// <param name="ultimateCounterparty">The ultimate counterparty information (required)</param>
        public SubMerchants(UltimateCounterpartyUnion ultimateCounterparty)
        {
            UltimateCounterparty = ultimateCounterparty;
        }

        /// <summary>
        /// Gets the ultimate counterparty information
        /// </summary>
        public UltimateCounterpartyUnion UltimateCounterparty { get; }
    }

    /// <summary>
    /// Ultimate counterparty types for sub-merchant information
    /// </summary>
    public static class UltimateCounterparty
    {
        /// <summary>
        /// Represents a business division as the ultimate counterparty
        /// </summary>
        [JsonDiscriminator("business_division")]
        public record BusinessDivision : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="BusinessDivision"/> instance
            /// </summary>
            /// <param name="id">The identifier of the business division</param>
            /// <param name="name">The name of the business division</param>
            public BusinessDivision(string id, string name)
            {
                Id = id.NotNullOrWhiteSpace(nameof(id));
                Name = name.NotNullOrWhiteSpace(nameof(name));
            }

            /// <summary>
            /// Gets the type of ultimate counterparty
            /// </summary>
            public string Type => "business_division";

            /// <summary>
            /// Gets the identifier of the business division
            /// </summary>
            public string Id { get; }

            /// <summary>
            /// Gets the name of the business division
            /// </summary>
            public string Name { get; }
        }

        /// <summary>
        /// Represents a business client as the ultimate counterparty
        /// </summary>
        [JsonDiscriminator("business_client")]
        public record BusinessClient : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="BusinessClient"/> instance
            /// </summary>
            /// <param name="tradingName">The trading name of the business (required, max 70 chars)</param>
            /// <param name="commercialName">The commercial name of the business (optional, max 100 chars)</param>
            /// <param name="url">The website of the business (optional, max 100 chars)</param>
            /// <param name="mcc">The merchant category code (optional, 4 digits)</param>
            /// <param name="registrationNumber">The registration number (optional, max 35 chars, required if address not provided)</param>
            /// <param name="address">The address of the business (optional, required if registration number not provided)</param>
            public BusinessClient(
                string tradingName,
                string? commercialName = null,
                string? url = null,
                string? mcc = null,
                string? registrationNumber = null,
                Address? address = null)
            {
                TradingName = tradingName.NotNullOrWhiteSpace(nameof(tradingName));
                CommercialName = commercialName.NotEmptyOrWhiteSpace(nameof(commercialName));
                Url = url.NotEmptyOrWhiteSpace(nameof(url));
                Mcc = mcc.NotEmptyOrWhiteSpace(nameof(mcc));
                RegistrationNumber = registrationNumber.NotEmptyOrWhiteSpace(nameof(registrationNumber));
                Address = address;

                // Validate business rules
                if (TradingName?.Length > 70)
                    throw new ArgumentException("Trading name cannot exceed 70 characters", nameof(tradingName));
                
                if (CommercialName?.Length > 100)
                    throw new ArgumentException("Commercial name cannot exceed 100 characters", nameof(commercialName));
                
                if (Url?.Length > 100)
                    throw new ArgumentException("URL cannot exceed 100 characters", nameof(url));
                
                if (Mcc != null && (Mcc.Length != 4 || !Mcc.All(char.IsDigit)))
                    throw new ArgumentException("MCC must be exactly 4 digits", nameof(mcc));
                
                if (RegistrationNumber?.Length > 35)
                    throw new ArgumentException("Registration number cannot exceed 35 characters", nameof(registrationNumber));

                // Either registration number or address must be provided
                if (string.IsNullOrWhiteSpace(RegistrationNumber) && Address == null)
                    throw new ArgumentException("Either registration number or address must be provided");
            }

            /// <summary>
            /// Gets the type of ultimate counterparty
            /// </summary>
            public string Type => "business_client";

            /// <summary>
            /// Gets the trading name of the business (max 70 characters)
            /// </summary>
            public string TradingName { get; } = default!;

            /// <summary>
            /// Gets the commercial name of the business (max 100 characters)
            /// </summary>
            public string? CommercialName { get; }

            /// <summary>
            /// Gets the website of the business (max 100 characters)
            /// </summary>
            public string? Url { get; }

            /// <summary>
            /// Gets the merchant category code (4 digits)
            /// </summary>
            public string? Mcc { get; }

            /// <summary>
            /// Gets the registration number of the business (max 35 characters)
            /// </summary>
            public string? RegistrationNumber { get; }

            /// <summary>
            /// Gets the address of the business
            /// </summary>
            public Address? Address { get; }
        }
    }
}