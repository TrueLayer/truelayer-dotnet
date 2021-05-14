using System.Threading.Tasks;

namespace TrueLayer
{
    public interface ITokenCache
    {
        TrueLayerToken PaymentToken { get; }
        Task SetPaymentToken(string payload, int expiresIn, string scope, string type);
    }
}
