using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    public static class SchemeIdentifier
    {
        [JsonDiscriminator("sort_code_account_number")]
        public record SortCodeAccountNumber : IDiscriminated
        {
            public SortCodeAccountNumber(string sortCode, string accountNumber)
            {
                SortCode = sortCode.NotNullOrWhiteSpace(nameof(sortCode));
                AccountNumber = accountNumber.NotNullOrWhiteSpace(nameof(accountNumber));
            }

            public string Type => "sort_code_account_number";
            public string SortCode { get; }
            public string AccountNumber { get; }
        }
    }
}
