using System.Collections.Generic;

namespace TrueLayer.PaymentsProviders.Model;

/// <summary>
/// Represents the complete set of capabilities supported by a payment provider.
/// </summary>
/// <param name="Payments">The payment capabilities supported by the provider.</param>
/// <param name="Mandates">The mandate capabilities supported by the provider.</param>
public record Capabilities(PaymentsCapabilities? Payments, MandatesCapabilities? Mandates);

/// <summary>
/// Represents payment capabilities for different payment types.
/// </summary>
/// <param name="BankTransfer">The bank transfer payment capabilities.</param>
public record PaymentsCapabilities(BankTransferCapabilities? BankTransfer);

/// <summary>
/// Represents mandate capabilities for Variable Recurring Payments (VRP).
/// </summary>
/// <param name="VrpSweeping">The VRP sweeping mandate capabilities.</param>
/// <param name="VrpCommercial">The VRP commercial mandate capabilities.</param>
public record MandatesCapabilities(VrpSweepingCapabilities? VrpSweeping, VrpCommercialCapabilities? VrpCommercial);

/// <summary>
/// Represents bank transfer payment capabilities.
/// </summary>
/// <param name="ReleaseChannel">The release channel for bank transfer payments.</param>
/// <param name="Schemes">The payment schemes supported for bank transfers.</param>
public record BankTransferCapabilities(string ReleaseChannel, IEnumerable<Scheme> Schemes);

/// <summary>
/// Represents VRP sweeping mandate capabilities.
/// </summary>
/// <param name="ReleaseChannel">The release channel for VRP sweeping mandates.</param>
public record VrpSweepingCapabilities(string ReleaseChannel);

/// <summary>
/// Represents VRP commercial mandate capabilities.
/// </summary>
/// <param name="ReleaseChannel">The release channel for VRP commercial mandates.</param>
public record VrpCommercialCapabilities(string ReleaseChannel);