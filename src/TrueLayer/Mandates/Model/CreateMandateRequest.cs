using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Payments.Model;
using static TrueLayer.Mandates.Model.Mandate;

namespace TrueLayer.Mandates.Model
{
    using MandateUnion = OneOf<VRPCommercialMandate, VRPSweepingMandate>;

    public record CreateMandateRequest(
        MandateUnion Mandate,
        string Currency,
        PaymentUserRequest User,
        Constraints Constraints,
        Dictionary<string, string> Metadata);
}
