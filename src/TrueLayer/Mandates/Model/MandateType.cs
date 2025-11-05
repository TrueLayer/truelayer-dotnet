namespace TrueLayer.Mandates.Model;

using System;

/// <summary>
/// Types of Variable Recurring Payment (VRP) mandates
/// </summary>
public enum MandateType
{
    /// <summary>
    /// Sweeping mandate for automated account sweeping
    /// </summary>
    Sweeping,

    /// <summary>
    /// Commercial mandate for business payments
    /// </summary>
    Commercial
}

/// <summary>
/// Extension methods for <see cref="MandateType"/>
/// </summary>
public static class MandateTypeExtensions
{
    /// <summary>
    /// Converts the mandate type to its string representation
    /// </summary>
    /// <param name="mandateType">The mandate type to convert</param>
    /// <returns>The string representation of the mandate type</returns>
    public static string AsString(this MandateType mandateType) =>
        mandateType switch
        {
            MandateType.Sweeping => "sweeping",
            MandateType.Commercial => "commercial",
            _ => throw new ArgumentException($"Invalid mandate type {mandateType}")
        };
}
