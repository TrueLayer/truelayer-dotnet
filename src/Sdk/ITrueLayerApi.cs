using TrueLayer.Auth;
using TrueLayer.Payments;

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
    }
}
