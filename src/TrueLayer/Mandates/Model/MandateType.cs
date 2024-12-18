namespace TrueLayer.Mandates.Model;

using System;

public enum MandateType
{
    Sweeping,
    Commercial
}

public static class MandateTypeExtensions
{
    public static string AsString(this MandateType mandateType) =>
        mandateType switch
        {
            MandateType.Sweeping => "sweeping",
            MandateType.Commercial => "commercial",
            _ => throw new ArgumentException($"Invalid mandate type {mandateType}")
        };
}
