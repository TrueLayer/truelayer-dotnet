using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Models
{
    public record ProviderAvailability(
        string RecommendedStatus,
        float ErrorRate,
        DateTime updatedAt
    );
}
