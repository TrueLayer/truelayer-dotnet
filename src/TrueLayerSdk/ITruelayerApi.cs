using TrueLayerSdk.Payments;

namespace TrueLayerSdk
{
    /// <summary>
    /// Convenience interface that provides access to the available Truelayer.com APIs.
    /// </summary>
    public interface ITruelayerApi
    {
        /// <summary>
        /// Gets the Payments API.
        /// </summary>
        IPaymentsClient Payments { get; }
    }
}
