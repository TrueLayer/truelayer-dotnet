using TrueLayer.Auth;
using TrueLayer.Payments;
using TrueLayer.Merchants;

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
        
        IMerchantsApi Merchants { get; }
    }
}
