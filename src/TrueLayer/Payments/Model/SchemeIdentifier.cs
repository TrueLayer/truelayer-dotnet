using System.Text.Json.Serialization;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    public static class SchemeIdentifier
    {
        [JsonDiscriminator(SortCodeAccountNumber.Discriminator)]
        public record SortCodeAccountNumber : IDiscriminated
        {
            public const string Discriminator = "sort_code_account_number";

            public SortCodeAccountNumber(string sortCode, string accountNumber)
            {
                SortCode = sortCode.NotNullOrWhiteSpace(nameof(sortCode));
                AccountNumber = accountNumber.NotNullOrWhiteSpace(nameof(accountNumber));
            }

            public string Type => Discriminator;
            public string SortCode { get; }
            public string AccountNumber { get; }
        }

        [JsonDiscriminator(Nrb.Discriminator)]
        public record Nrb : IDiscriminated
        {
            public const string Discriminator = "nrb";


            public Nrb(string value)
            {
                Value = value.NotNullOrWhiteSpace(nameof(value));
            }

            public string Type => Discriminator;

            [JsonPropertyName(Discriminator)]
            public string Value { get; }
        }

        [JsonDiscriminator(Bban.Discriminator)]
        public record Bban : IDiscriminated
        {
            public const string Discriminator = "bban";

            public Bban(string value)
            {
                Value = value.NotNullOrWhiteSpace(nameof(value));
            }

            public string Type => Discriminator;

            [JsonPropertyName(Discriminator)]
            public string Value { get; }
        }

        [JsonDiscriminator(Iban.Discriminator)]

        public record Iban : IDiscriminated
        {
            public const string Discriminator = "iban";

            public Iban(string value)
            {
                Value = value.NotNullOrWhiteSpace(nameof(value));
            }

            public string Type => Discriminator;

            [JsonPropertyName(Discriminator)]
            public string Value { get; }
        }
    }
}
