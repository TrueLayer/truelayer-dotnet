using System.Text.Json.Serialization;
using TrueLayer.Serialization;

namespace TrueLayer.Payouts.Model
{
    /// <summary>
    /// Account Identifier types
    /// </summary>
    public static class AccountIdentifier
    {

     [JsonDiscriminator(SortCodeAccountNumber.Discriminator)]
     public record SortCodeAccountNumber : IDiscriminated
     {
         public string Type => "sort_code_account_number";
    
         public string SortCode { get; }
    
         public string AccountNumber { get; }
    
         public const string Discriminator = "sort_code_account_number";
    
         public SortCodeAccountNumber(string sortCode, string accountNumber)
         {
             SortCode = sortCode.VerifyNotNull("sortCode");
             AccountNumber = accountNumber.VerifyNotNull("accountNumber");
         }
    
         protected SortCodeAccountNumber(SortCodeAccountNumber original)
         {
             SortCode = original.SortCode;
             AccountNumber = original.AccountNumber;
         }
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
