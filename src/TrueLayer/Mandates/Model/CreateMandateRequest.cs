using System.Collections.Generic;
using OneOf;
using TrueLayer.Payments.Model;
using static TrueLayer.Mandates.Model.Mandate;

namespace TrueLayer.Mandates.Model
{
    using MandateUnion = OneOf<VRPCommercialMandate, VRPSweepingMandate>;

    /// <summary>
    /// Creates a new <see cref="CreateMandateRequest"/>
    /// </summary>
    /// <param name="Mandate">Either a Commercial or Sweeping mandate.>
    /// <param name="Currency">Three-letter ISO currency code</param>
    /// <param name="Constraints">Sets the limits for the payments that can be created by the mandate. If a payment is attempted that doesn't fit within these constraints it will fail.</param>
    /// <param name="User">Details of the end user who is making the payment. Whether or not these fields are required depends on whether you are using your own PISP licence (if you are, these fields are not required).</param>
    /// <param name="Metadata">Optional field for adding custom key-value data to a resource. This object can contain a maximum of 10 key-value pairs, each with a key with a maximum length of 40 characters and a non-null value with a maximum length of 500 characters.</param>
    public record CreateMandateRequest(
        MandateUnion Mandate,
        string Currency,
        Constraints Constraints,
        PaymentUserRequest? User = null,
        Dictionary<string, string>? Metadata = null);
}
