using TrueLayer.Payments;

namespace TrueLayer
{
    public interface ITrueLayerClient
    {
        IPaymentsApi Payments { get; }
    }
}
