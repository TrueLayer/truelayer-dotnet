using TrueLayer.Auth;
using TrueLayer.Payments;

namespace TrueLayer
{
    public interface ITrueLayerClient
    {
        IAuthApi Auth { get; }
        IPaymentsApi Payments { get; }
    }
}
