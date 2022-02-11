using TrueLayer.Payments.Model;

namespace TrueLayer.MerchantAccounts.Model
{
    /// <summary>
    /// Represents a collection of payment source details.
    /// </summary>
    /// <param name="Items">Details of the payment sources.</param>
    public record GetPaymentSourcesResponse(PaymentSource[] Items);
}
