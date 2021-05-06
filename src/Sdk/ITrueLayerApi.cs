using TrueLayer.Auth;
using TrueLayer.Payments;
using TrueLayer.Payouts;

namespace TrueLayer
{
    /// <summary>
    /// Convenience interface that provides access to the available TrueLayer.com APIs.
    /// </summary>
    public interface ITrueLayerApi
    {
        /// <summary>
        /// Gets the Auth API.
        /// </summary>
        IAuthClient Auth { get; }
        
        /// <summary>
        /// Gets the Payments API.
        /// </summary>
        IPaymentsClient Payments { get; }

        /// <summary>
        /// Gets the Payouts API.
        /// </summary>
        IPayoutsClient Payouts { get; }
    }
}
