using System.Collections.Generic;

namespace TrueLayer.Payments.Model
{
    public record GetProvidersRequest
    {
        public GetProvidersRequest(string clientId, List<string> authFlowType, List<string> accountType, List<string> currency, 
            List<string>? country = default, List<string>? additionalInputType = default, string? releaseChannel = default)
        {
            clientId.NotNullOrWhiteSpace(nameof(clientId));
            authFlowType.NotNull(nameof(authFlowType)).NotEmpty(nameof(authFlowType));
            accountType.NotNull(nameof(accountType)).NotEmpty(nameof(authFlowType));
            currency.NotNull(nameof(currency)).NotEmpty(nameof(authFlowType));

            ClientId = clientId;
            AuthFlowType = authFlowType;
            AccountType = accountType;
            Currency = currency;

            Country = country;
            AdditionalInputType = additionalInputType;
            ReleaseChannel = releaseChannel;
        }

        /// <summary>
        /// Your client ID. Required for verifying access to providers.
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// Will only return providers with schemes that support the listed auth flows.
        /// Refer to <see cref="Constants.AuthFlowType"/>
        /// </summary>
        public List<string> AuthFlowType { get; }

        /// <summary>
        /// Will only return providers with schemes that support the listed account identifiers.
        /// Refer to <see cref="Constants.AccountType"/>
        /// </summary>
        public List<string> AccountType { get; }

        /// <summary>
        /// ISO 4217 currency codes: e.g. GBP,EUR. 
        /// Will only return providers with schemes that support the listed currencies.
        /// </summary>
        public List<string> Currency { get; }

        /// <summary>
        /// ISO 3166-1 alpha-2 country codes: e.g. GB,FR. 
        /// Will only return providers from the listed countries.
        /// </summary>
        public List<string>? Country { get; }

        /// <summary>
        /// Will filter out any provider schemes that require additional_inputs field types not specified 
        /// (including any on subsequent inputs as part of the embedded flow). 
        /// If omitted, will only return provider schemes that have no required additional inputs.
        /// Refer to <see cref="Constants.AdditionalInputType"/>
        /// </summary>
        public List<string>? AdditionalInputType { get; }

        /// <summary>
        /// Will filter out any providers that are not on the specified release channel 
        /// (public_beta also includes all those in live, and private_beta also includes all those in public_beta). 
        /// If omitted, will only return providers that are generally available (equivalent to live).
        /// Refer to <see cref="Constants.ReleaseChannel"/>
        /// </summary>
        public string? ReleaseChannel { get; }
    }
}
