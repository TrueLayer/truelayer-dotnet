namespace TrueLayer.PayDirect.Model
{
    /// <summary>
    /// Codes that describe the reason for the withdrawal
    /// </summary>
    public static class ContextCodes
    {
        /// <summary>
        /// The payout is allowing an end user to withdraw funds from your service. 
        /// For example, this code would be used if you are an iGaming company allowing users to withdraw their winnings.
        /// </summary>
        public const string Withdrawal = "withdrawal";
        
        /// <summary>
        /// The payout is being used to pay for a service. 
        /// For example, you are a Gig Economy company paying a driver or you are a platform paying a merchant.
        /// </summary>
        public const string ServicePayment = "service_payment";
        
        /// <summary>
        /// You are paying yourself. Use this code to withdraw your funds from your account with us.
        /// /// </summary>
        public const string Internal = "internal";
    }
}
