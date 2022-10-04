using System.Collections.Generic;

namespace TrueLayer.PaymentsProviders.Model
{
    public record Capabilities(PaymentsCapabilities Payments, MandatesCapabilities Mandates);

    public record PaymentsCapabilities(BankTransferCapabilities BankTransfer);

    public record MandatesCapabilities(VrpSweepingCapabilities VrpSweeping, VrpCommercialCapabilities VrpCommercial);

    public record BankTransferCapabilities(string ReleaseChannel, IEnumerable<Scheme> Schemes);

    public record VrpSweepingCapabilities(string ReleaseChannel);

    public record VrpCommercialCapabilities(string ReleaseChannel);
}
