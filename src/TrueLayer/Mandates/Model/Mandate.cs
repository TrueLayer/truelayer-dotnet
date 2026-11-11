using System;
using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Mandates.Model.Beneficiary;

namespace TrueLayer.Mandates.Model;

using BeneficiaryUnion = OneOf<ExternalAccount, MerchantAccount>;
using ProviderUnion = OneOf<CreateProviderSelection.UserSelected, CreateProviderSelection.Preselected>;

/// <summary>
/// Contains mandate types for Variable Recurring Payments (VRP).
/// </summary>
public static class Mandate
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="Type">The type of VRP mandate that will be created.</param>
    /// <param name="UseCase">The use case for which the cVRP mandate is used for.</param>
    /// <param name="ProviderSelection">Provider selection.</param>
    /// <param name="Beneficiary">Represents a beneficiary account.</param>
    /// <param name="StatementReference">A custom reference for the mandate, available for regulated customers only. Pattern: ^[a-zA-Z0-9-:()\.,'\+ \?\/]{1,18}$. If not specified, one is automatically set to be the clients name, adjusted as needed.</param>
    /// <param name="Reference">Deprecated: use <paramref name="StatementReference"/> instead. A custom reference for the mandate, available for regulated customers only. Pattern: ^[a-zA-Z0-9-:()\.,'\+ \?\/]{1,18}$.</param>
    [JsonDiscriminator("commercial")]
    public record VRPCommercialMandate(
        string Type,
        CommercialMandateUseCase UseCase,
        ProviderUnion ProviderSelection,
        BeneficiaryUnion Beneficiary,
        string? StatementReference = null,
        [property: Obsolete("Use StatementReference instead.")] string? Reference = null) : IDiscriminated;

    /// <summary>
    ///
    /// </summary>
    /// <param name="Type">The type of VRP mandate that will be created.</param>
    /// <param name="ProviderSelection">Provider selection.</param>
    /// <param name="Beneficiary">Represents a beneficiary account.</param>
    /// <param name="Reference"></param>
    [JsonDiscriminator("sweeping")]
    public record VRPSweepingMandate(
        string Type,
        ProviderUnion ProviderSelection,
        BeneficiaryUnion Beneficiary,
        string? Reference = null) : IDiscriminated;
}
