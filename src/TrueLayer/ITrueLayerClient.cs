using TrueLayer.Auth;
using TrueLayer.MerchantAccounts;
using TrueLayer.Payments;
using TrueLayer.PaymentsProviders;
using TrueLayer.Payouts;

namespace TrueLayer
{
    using TrueLayer.Mandates;

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
        /// Gets the Payments Providers API resource
        /// </summary>
        IPaymentsProvidersApi PaymentsProviders { get; }

        /// <summary>
        /// Gets the Payouts API resource
        /// </summary>
        IPayoutsApi Payouts { get; }

        /// <summary>
        /// Gets the Merchant Accounts API resource
        /// </summary>
        IMerchantAccountsApi MerchantAccounts { get; }

        /// <summary>
        /// Gets the Mandates API resource
        /// </summary>
        IMandatesApi Mandates { get; }
    }
}
