using System.Collections.Generic;
using TrueLayer.Payments.Model.AuthFlowModel;

namespace TrueLayer.Payments.Model
{
    public record GetProvidersResponse(List<Result>? Results);

    public class CurrencyRequirements
    {
        /// <summary>
        /// A list of ISO 4217 currency codes supported by the provider scheme.
        /// </summary>
        public List<string>? SupportedCurrencies { get; set; }
    }

    public class StringFieldRequirements
    {
        /// <summary>
        /// The corresponding field on the request is mandatory.
        /// </summary>
        public bool Mandatory { get; set; }
        /// <summary>
        /// The corresponding field on the request must be at least this many characters.
        /// </summary>
        public int MinLength { get; set; }
        /// <summary>
        /// The corresponding field on the request must be at most this many characters.
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// The corresponding field on the request must match the regular expression.
        /// </summary>
        public string? Regex { get; set; }
        /// <summary>
        /// A well known format to help you communicate to your client how the field will be validated.
        /// See <see cref="Constants.StringFormat"/>
        /// </summary>
        public string? Format { get; set; }
    }

    public class AccountRequirements
    {
        /// <summary>
        /// A list of account types supported by the scheme.
        /// See <see cref="Constants.AccountType"/>
        /// </summary>
        public List<string>? Types { get; set; }
    }
    public class ParticipantRequirements
    {
        /// <summary>
        /// Account requirements.
        /// </summary>
        public AccountRequirements? Account { get; set; }
        /// <summary>
        /// An object detailing the requirements for the name, 
        /// if omitted then setting the name on the initiation request is not supported for this scheme.
        /// </summary>
        public StringFieldRequirements? Name { get; set; }
        /// <summary>
        /// If a remitter is required. Not present on beneficiary as that is always mandatory.
        /// </summary>
        public bool Mandatory { get; set; }
    }

    public class SeparateReference
    {
        /// <summary>
        /// Specifies the requirements for the beneficiary reference string for the payment.
        /// </summary>
        public StringFieldRequirements? Beneficiary { get; set; }
        /// <summary>
        /// Specifies the requirements for the remitter reference string for the payment.
        /// </summary>
        public StringFieldRequirements? Remitter { get; set; }
    }

    public class ReferencesRequirements
    {
        /// <summary>
        /// A list of supported reference types.
        /// See <see cref="Constants.ReferenceType"/>
        /// </summary>
        public List<string>? Types { get; set; }
        /// <summary>
        /// Will be set if single is supported. Specifies the requirements for a single reference string for the payment.
        /// </summary>
        public StringFieldRequirements? Single { get; set; }
        /// <summary>
        /// Will be set if separate is supported.
        /// </summary>
        public SeparateReference? Separate { get; set; }
    }

    public class SingleImmediatePaymentRequirement
    {
        /// <summary>
        /// Which currencies are supported.
        /// </summary>
        public CurrencyRequirements? Currency { get; set; }
        /// <summary>
        /// Which beneficiary details are required/supported.
        /// </summary>
        public ParticipantRequirements? Beneficiary { get; set; }
        /// <summary>
        /// Which remitter details are required/supported. If omitted, the scheme does not support receiving remitter details.
        /// </summary>
        public ParticipantRequirements? Remitter { get; set; }
        /// <summary>
        /// Which reference types are required/supported.
        /// </summary>
        public ReferencesRequirements? References { get; set; }
    }

    public class Requirement
    {
        /// <summary>
        /// Which auth flows are supported and what fields are required/available to initiate a payment for those flows.
        /// </summary>
        public AuthFlowRequirement? AuthFlow { get; set; }
        public SingleImmediatePaymentRequirement? SingleImmediatePayment { get; set; }
    }

    public class FeeOption
    {
        /// <summary>
        /// A unique identifier for the fee option, shared across providers. 
        /// This should be provided on the single_immediate_payment.fee_option_id field on the initiation request.
        /// </summary>
        public string? FeeOptionId { get; set; }
        /// <summary>
        /// A value to indicate if the beneficiary would pay any fees on the transaction, when using this fee option.
        /// Refer to <see cref="Constants.ReleaseChannel"/>
        /// </summary>
        public string? BeneficiaryFee { get; set; }
        /// <summary>
        /// A value to indicate if the remitter would pay any fees on the transaction, when using this fee option.
        /// Refer to <see cref="Constants.ReleaseChannel"/>
        /// </summary>
        public string? RemitterFee { get; set; }
    }

    public class SingleImmediatePaymentScheme
    {
        /// <summary>
        /// A unique identifier for the scheme, shared across providers. 
        /// This should be provided on the single_immediate_payment.scheme_id field on the initiation request.
        /// </summary>
        public string? SchemeId { get; set; }
        /// <summary>
        /// Contains at least one set of requirements, one of which should be adhered to 
        /// in order to create a valid initiation request for the scheme.
        /// </summary>
        public List<Requirement>? Requirements { get; set; }
        /// <summary>
        /// If present, then fees are payable to make a payment on the scheme,
        /// and you must choose an option in this list when initiating a payment.
        /// </summary>
        public List<FeeOption>? FeeOptions { get; set; }
    }

    public class Result
    {
        /// <summary>
        /// This is the provider ID that you will send us in the create payment request.
        /// </summary>
        public string? ProviderId { get; set; }
        /// <summary>
        /// This is the address of the logo asset in SVG form.
        /// </summary>
        public string? LogoUrl { get; set; }
        /// <summary>
        /// This is the address of the icon asset in SVG form.
        /// </summary>
        public string? IconUrl { get; set; }
        /// <summary>
        /// This is a readable name for the provider.
        /// </summary>
        public string? DisplayName { get; set; }
        /// <summary>
        /// The ISO 3166-1 alpha-2 country code for the provider.
        /// </summary>
        public string? Country { get; set; }
        /// <summary>
        /// This array includes all divisions that are available on this provider: 
        /// e.g. retail and business would indicate you can use this single provider to access both retail 
        /// and business accounts.
        /// </summary>
        public List<string>? Divisions { get; set; }
        /// <summary>
        /// The array lists the available schemes the provider can use, within each is described the requirements 
        /// for the fields on the Single Immediate Payment initiation request.
        /// </summary>
        public List<SingleImmediatePaymentScheme>? SingleImmediatePaymentSchemes { get; set; }
        /// <summary>
        /// This indicates the product maturity of the provider: live and public_beta providers are visible to everyone, 
        /// while private_beta providers must be enabled for the client_id provided in the query parameters to be visible.
        /// Refer to <see cref="Constants.ReleaseChannel"/>
        /// </summary>
        public string? ReleaseStage { get; set; }
    }
}
