namespace TrueLayer.Payments.Model
{
    public record SortCodeAccountNumberSchemeIdentifier : IDiscriminated
    {
        public SortCodeAccountNumberSchemeIdentifier(string sortCode, string accountNumber)
        {
            SortCode = sortCode.NotNullOrWhiteSpace(nameof(sortCode));
            AccountNumber = accountNumber.NotNullOrWhiteSpace(nameof(accountNumber));
        }

        public string Type => "sort_code_account_number";
        public string SortCode { get; }
        public string AccountNumber { get; }
    }
}
