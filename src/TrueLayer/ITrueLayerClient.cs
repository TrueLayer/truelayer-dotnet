using TrueLayer.Auth;
using TrueLayer.Payments;
using TrueLayer.MerchantAccounts;

namespace TrueLayer
{
    /// <summary>
    /// Provides access to TrueLayer APIs
    /// </summary>
    public interface ITrueLayerClient
    {
        /// <summary>
        /// Gets the Authorization API resource
        /// </summary>
        IAuthApi Auth { get; }
        
        /// <summary>
        /// Gets the Payments API resource
        /// </summary>
        IPaymentsApi Payments { get; }

        /// <summary>
        /// Gets the Merchant Accounts API resource
        /// </summary>
        IMerchantAccountsApi MerchantAccounts { get; }
    }
}
