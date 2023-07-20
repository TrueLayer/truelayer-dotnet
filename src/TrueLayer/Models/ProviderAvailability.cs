using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Models
{
    public enum RecommendedStatus { [EnumMember(Value = "healthy")] Healthy = 0, [EnumMember(Value = "unhealthy")] Unhealthy = 1 }

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
}
