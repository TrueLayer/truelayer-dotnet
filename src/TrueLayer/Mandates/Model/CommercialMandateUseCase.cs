namespace TrueLayer.Mandates.Model;

/// <summary>
/// The use case for which the commercial VRP (cVRP) mandate is used for.
/// </summary>
public enum CommercialMandateUseCase
{
    /// <summary>
    /// Charity payments
    /// </summary>
    Charity,

    /// <summary>
    /// Financial services payments
    /// </summary>
    FinancialServices,

    /// <summary>
    /// Government payments
    /// </summary>
    Government,

    /// <summary>
    /// Utilities payments
    /// </summary>
    Utilities
}
