using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Models
{
    internal enum RecommendedStatus { Healthy = 0, Unhealthy = 1 }

    /// <summary>
    /// Provider Availability object
    /// </summary>
    /// <param name="RecommendedStatus"></param>
    /// <param name="UpdatedAt">The point in time when this data was collected. Value is in UTC.</param>
    public record ProviderAvailability(
        string RecommendedStatus,
        DateTime UpdatedAt
    );
}
