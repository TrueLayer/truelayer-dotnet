namespace TrueLayer.Payments.Model
{
    using SchemeIdentifierUnion = Union<SortCodeAccountNumberSchemeIdentifier>;

    /// <summary>
    /// Represents a TrueLayer beneficiary merchant account
    /// </summary>
    public sealed class MerchantAccountBeneficiary : IDiscriminated
    {
        /// <summary>
        /// Creates a new <see cref="MerchantAccountBeneficiary"/>
        /// </summary>
        /// <param name="id">Your TrueLayer merchant account identifier</param>
        /// <returns></returns>
        internal MerchantAccountBeneficiary(string id)
        {
            Id = id.NotNullOrWhiteSpace(nameof(id));
        }

        public string Type => "merchant_account";

        /// <summary>
        /// Gets the TrueLayer merchant account identifier
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The name of the beneficiary. 
        /// If unspecified, the API will use the account owner name associated to the selected merchant account.
        /// </summary>
        public string? Name { get; set; }
    }

    /// <summary>
    /// Represents an external beneficiary account
    /// </summary>
    public sealed class ExternalAccountBeneficiary : IDiscriminated
    {
        public ExternalAccountBeneficiary(string name, string reference, SchemeIdentifierUnion schemeIdentifier)
        {
            Name = name.NotNullOrWhiteSpace(nameof(name));
            Reference = reference.NotNullOrWhiteSpace(nameof(reference));
            SchemeIdentifier = schemeIdentifier;
        }

        public string Type => "external";

        /// <summary>
        /// Gets the name of the external account holder
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the reference for the external bank account holder 
        /// </summary>
        /// <value></value>
        public string Reference { get; }

        /// <summary>
        /// Gets the unique scheme identifier for the external account
        /// </summary>
        /// <value></value>
        public SchemeIdentifierUnion SchemeIdentifier { get; }
    }
}
