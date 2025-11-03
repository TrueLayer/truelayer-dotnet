using System;

namespace TrueLayer.Models;

internal enum RecommendedStatus { Healthy = 0, Unhealthy = 1 }

/// <summary>
/// Provider Availability object
/// </summary>
/// <param name="RecommendedStatus"></param>
/// <param name="ErrorRate">A ratio between the number of provider errors and all requests for the provider.</param>
/// <param name="UpdatedAt">The point in time when this data was collected. Value is in UTC.</param>
public record ProviderAvailability(
    string RecommendedStatus,
    float ErrorRate,
    DateTime UpdatedAt
);