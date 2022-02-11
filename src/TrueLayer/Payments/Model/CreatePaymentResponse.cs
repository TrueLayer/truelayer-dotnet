namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Create Payment Response type
    /// </summary>
    /// <param name="Id">The unique identifier of the payment</param>
    /// <param name="ResourceToken">The token used to complete the payment via a front-end channel</param>
    /// <param name="User">The end user details</param>
    public record CreatePaymentResponse(string Id, string ResourceToken, PaymentUserResponse User);
}
