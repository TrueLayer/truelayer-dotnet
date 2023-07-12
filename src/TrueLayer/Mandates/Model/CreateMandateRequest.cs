using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Payments.Model;

namespace TrueLayer.Mandates.Model
{
    using Mandate = OneOf<VRPCommercialMandate, VRPSweepingMandate>;

    public record CreateMandateRequest(
        Mandate Mandate,
        string Currency,
        PaymentUserRequest User,
        Constraints Constraints,
        Dictionary<string, string> Metadata);
}
