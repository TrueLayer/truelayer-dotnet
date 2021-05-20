namespace TrueLayer.PayDirect.Model
{
    /// <summary>
    /// The type of account
    /// </summary>
    public static class AccountIdentifierTypes
    {
        /// <summary>
        /// The account is identified by the combination of sort code and account number, typically UK accounts
        /// </summary>
        public const string SortCodeAccountNumber = "sort_code_account_number";
        
        /// <summary>
        /// The account is identified using its International Bank Account Number
        /// </summary>
        public const string Iban = "iban";
        
        /// <summary>
        /// The account is identified using its country specific Basic Bank Account Number
        /// </summary>
        public const string Bban = "bban";
        
        /// <summary>
        /// The account is identified using the NRB format
        /// </summary>
        public const string Nrb = "nrb";
    }
}
