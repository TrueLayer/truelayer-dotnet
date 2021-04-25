using TrueLayerSdk.Auth;
using TrueLayerSdk.Data;
using TrueLayerSdk.Payments;

namespace TrueLayerSdk
{
    /// <summary>
    /// Convenience interface that provides access to the available Truelayer.com APIs.
    /// </summary>
    public interface ITruelayerApi
    {
        /// <summary>
        /// Gets the Auth API.
        /// </summary>
        public IAuthClient Auth { get; }
        
        /// <summary>
        /// Gets the Data API.
        /// </summary>
        public IDataClient Data { get; }
        
        /// <summary>
        /// Gets the Payments API.
        /// </summary>
        IPaymentsClient Payments { get; }
    }
}
