namespace TrueLayer.Payments.Model
{
    public interface ISchemeIdentifier : IDiscriminated
    {
        string Type { get; }
    }

    public static class SchemeIdentifier
    {
        public static SortCodeAccountNumberSchemeIdentifier SortCodeAccountNumber(string sortCode, string accountNumber)
            => new SortCodeAccountNumberSchemeIdentifier(sortCode, accountNumber);
    }

    public record SortCodeAccountNumberSchemeIdentifier : ISchemeIdentifier
    {
        internal SortCodeAccountNumberSchemeIdentifier(string sortCode, string accountNumber)
        {
            SortCode = sortCode.NotNullOrWhiteSpace(nameof(sortCode));
            AccountNumber = accountNumber.NotNullOrWhiteSpace(nameof(accountNumber));
        }

        public string Type => "sort_code_account_number";
        public string SortCode { get; }
        public string AccountNumber { get; }
    }
}
