using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Payments.Model.AccountIdentifier;

namespace TrueLayer.Mandates.Model
{
    using AccountIdentifier = OneOf<SortCodeAccountNumber, Iban, Bban, Nrb>;

    /// <summary>
    /// Represents a TrueLayer beneficiary account
    /// </summary>
    public static class Beneficiary
    {
        /// <summary>
        /// Creates a new <see cref="MerchantAccount"/>
        /// </summary>
        /// <param name="Type">Type of beneficiary.</param>
        /// <param name="MerchantAccountId">TrueLayer merchant account identifier.</param>
        /// <param name="AccountHolderName">Name of the beneficiary.</param>
        [JsonDiscriminator("merchant_account")]
        public record MerchantAccount(
            string Type,
            string MerchantAccountId,
            string? AccountHolderName = null) : IDiscriminated;

        /// <summary>
        /// Represents an external beneficiary account
        /// </summary>
        /// <param name="Type">Type of beneficiary.</param>
        /// <param name="AccountHolderName">Name of the beneficiary.</param>
        /// <param name="AccountIdentifier">Unique identifier for the external account.</param>
        [JsonDiscriminator("external_account")]
        public record ExternalAccount(
            string Type,
            string AccountHolderName,
            AccountIdentifier AccountIdentifier) : IDiscriminated;
    }
}
