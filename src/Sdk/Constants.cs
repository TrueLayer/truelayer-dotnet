namespace TrueLayer
{
    public static class Constants
    {
        public static class ReleaseChannel
        {
            public const string Live = "live";
            public const string PublicBeta = "public_beta";
            public const string PrivateBeta = "private_beta";
        }

        public static class AdditionalInputType
        {
            public const string Text = "text";
            public const string TextWithImage = "text_with_image";
            public const string Select = "select";
        }

        public static class AccountType
        {
            public const string SortCodeAccountNumber = "sort_code_account_number";
            public const string Iban = "iban";
            public const string Bban = "bban";
            public const string Nrb = "nrb";
        }

        public static class AuthFlowType
        {
            public const string Redirect = "redirect";
            public const string Embedded = "embedded";
        }

        public static class Fee
        {
            public const string Free = "free";
            public const string Payable = "payable";
            public const string Unknown = "unknown";
        }

        public static class ReferenceType
        {
            public const string Single = "single";
            public const string Separate = "separate";
        }

        public static class StringFormat
        {
            public const string Any = "any";
            public const string Numerical = "numerical";
            public const string Alphabetical = "alphabetical";
            public const string Alphanumerical = "alphanumerical";
            public const string Email = "email";
        }
    }
}
