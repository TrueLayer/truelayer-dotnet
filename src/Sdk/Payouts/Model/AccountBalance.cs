namespace TrueLayer.Payouts.Model
{
    public record AccountBalance
    {
        /// <summary>
        /// The currency of the account. Accounts can only hold 1 currency each.
        /// </summary>
        public string Currency { get; init; } = null!;
        
        /// <summary>
        /// The IBAN of the account that can be used to send funds to the account.
        /// </summary>
        public string Iban { get; init; } = null!;
        
        /// <summary>
        /// Denotes whether the account is active or not, the field can be enabled or disabled.
        /// </summary>
        public string Status { get; init; } = null!;
        
        /// <summary>
        /// The current balance of the account in the smallest denomination including transactions that are pending or in transit.
        /// </summary>
        public long CurrentBalanceInMinor { get; init; }
        
        /// <summary>
        /// The balance available to spend in the smallest denomination.
        /// </summary>
        public long AvailableBalanceInMinor { get; init; }

        /// <summary>
        /// Gets whether the account is enabled
        /// </summary>
        public bool Enabled => Status == "enabled";
    }
}
