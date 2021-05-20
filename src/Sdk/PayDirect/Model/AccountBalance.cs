namespace TrueLayer.PayDirect.Model
{
    /// <summary>
    /// Defines a wallet account balance
    /// </summary>
    /// <param name="Currency">The currency of the account. Accounts can only hold 1 currency each.</param>
    /// <param name="Iban">The IBAN of the account that can be used to send funds to the account.</param>
    /// <param name="Status">Denotes whether the account is active or not, the field can be enabled or disabled.</param>
    /// <param name="AccountOwner">The name of the account owner</param>
    /// <param name="CurrentBalanceInMinor">The current balance of the account in the smallest denomination including transactions that are pending or in transit.</param>
    /// <param name="AvailableBalanceInMinor">The balance available to spend in the smallest denomination.</param>
    /// <returns></returns>
    public record AccountBalance(string Currency, string Iban, string Status, string AccountOwner, long CurrentBalanceInMinor, long AvailableBalanceInMinor)
    {
        /// <summary>
        /// Gets whether the account is enabled
        /// </summary>
        public bool Enabled => Status == "enabled";
    }
}
