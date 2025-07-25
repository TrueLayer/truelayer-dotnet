using System.Text.Json.Serialization;
using TrueLayer.Serialization;

namespace TrueLayer.Payouts.Model
{
    /// <summary>
    /// Account Identifier types
    /// </summary>
    public static class AccountIdentifier
    {
        /// <summary>
        /// Defines a bank account identified by an IBAN
        /// </summary>
        [JsonDiscriminator(Discriminator)]
        public record Iban : IDiscriminated
        {
            public const string Discriminator = "iban";

            /// <summary>
            /// Creates a new <see cref="Iban"/> instance
            /// </summary>
            /// <param name="value">
            /// Valid International Bank Account Number (no spaces).
            /// Consists of a 2 letter country code, followed by 2 check digits,
            /// and then by up to 30 alphanumeric characters (also known as the BBAN).
            /// </param>
            public Iban(string value)
            {
                Value = value.NotNullOrWhiteSpace(nameof(value));
            }

            /// <summary>
            /// Gets the scheme identifier type
            /// </summary>
            public string Type => Discriminator;

            /// <summary>
            /// Gets the IBAN value
            /// </summary>
            [JsonPropertyName(Discriminator)]
            public string Value { get; }
        }

        [JsonDiscriminator(Discriminator)]
        public record SortCodeAccountNumber : IDiscriminated
        {
            public const string Discriminator = "sort_code_account_number";

            /// <summary>
            /// Creates a new <see cref="SortCodeAccountNumber"/> instance.
            /// </summary>
            /// <param name="sortCode">6 digits sort code.</param>
            /// <param name="accountNumber">8 digits account number.</param>
            public SortCodeAccountNumber(string sortCode, string accountNumber)
            {
                SortCode = sortCode;
                AccountNumber = accountNumber;
            }

            /// <summary>
            /// Gets the scheme identifier type
            /// </summary>
            public string Type => Discriminator;

            /// <summary>
            /// Gets the sort code
            /// </summary>
            public string SortCode { get; }

            /// <summary>
            /// Gets the account number
            /// </summary>
            public string AccountNumber { get; }

        }

        /// <summary>
        /// Defines a bank account identified by a Polish NRB
        /// </summary>
        /// <value></value>
        [JsonDiscriminator(Discriminator)]
        public record Nrb : IDiscriminated
        {
            public const string Discriminator = "nrb";

            /// <summary>
            /// Creates a new <see cref="Nrb"/> instance
            /// </summary>
            /// <param name="value">
            /// Valid Polish NRB (no spaces).
            /// Consists of 2 check digits, followed by an 8 digit bank branch number, and then by a 16 digit bank account number.
            /// Equivalent to a Polish IBAN with the country code removed.
            /// </param>
            public Nrb(string value)
            {
                Value = value.NotNullOrWhiteSpace(nameof(value));
            }

            /// <summary>
            /// Gets the scheme identifier type
            /// </summary>
            public string Type => Discriminator;

            /// <summary>
            /// Gets the NRB value
            /// </summary>
            [JsonPropertyName(Discriminator)]
            public string Value { get; }
        }
    }
}
