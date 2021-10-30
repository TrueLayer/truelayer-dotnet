using System.Text.Json.Serialization;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Scheme Identifier types
    /// </summary>
    public static class SchemeIdentifier
    {
        /// <summary>
        /// Defines a bank account identified by sort code and account number
        /// </summary>
        /// <value></value>
        [JsonDiscriminator(SortCodeAccountNumber.Discriminator)]
        public record SortCodeAccountNumber : IDiscriminated
        {
            public const string Discriminator = "sort_code_account_number";

            /// <summary>
            /// Creates a new <see cref="SortCodeAccountNumber"/> instance
            /// </summary>
            /// <param name="sortCode">The bank account sort code</param>
            /// <param name="accountNumber">The bank account number</param>
            public SortCodeAccountNumber(string sortCode, string accountNumber)
            {
                SortCode = sortCode.NotNullOrWhiteSpace(nameof(sortCode));
                AccountNumber = accountNumber.NotNullOrWhiteSpace(nameof(accountNumber));
            }

            /// <summary>
            /// Gets the scheme identifier type
            /// </summary>
            public string Type => Discriminator;
            
            /// <summary>
            /// Gets the bank account sort code
            /// </summary>
            public string SortCode { get; }
            
            /// <summary>
            /// Gets the bank account number
            /// </summary>
            public string AccountNumber { get; }
        }

        /// <summary>
        /// Defines a bank account identified by a Polish NRB
        /// </summary>
        /// <value></value>
        [JsonDiscriminator(Nrb.Discriminator)]
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

        /// <summary>
        /// Defines a bank account identified by a BBAN
        /// </summary>
        [JsonDiscriminator(Bban.Discriminator)]
        public record Bban : IDiscriminated
        {
            public const string Discriminator = "bban";

            /// <summary>
            /// Creates a new <see cref="Bban"/> instance
            /// </summary>
            /// <param name="value">
            /// Valid Basic Bank Account Number (no spaces). 
            /// Consists of up to 30 alphanumeric characters, with a fixed length per country. 
            /// Forms the latter part of the IBAN as described above.
            /// </param>
            public Bban(string value)
            {
                Value = value.NotNullOrWhiteSpace(nameof(value));
            }

            /// <summary>
            /// Gets the scheme identifier type
            /// </summary>
            public string Type => Discriminator;

            /// <summary>
            /// Gets the BBAN value
            /// </summary>
            [JsonPropertyName(Discriminator)]
            public string Value { get; }
        }

        /// <summary>
        /// Defines a bank account identified by an IBAN
        /// </summary>
        [JsonDiscriminator(Iban.Discriminator)]
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
    }
}
