using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;

namespace TrueLayer.Mandates.Model
{
    public static class Provider
    {
        [JsonDiscriminator("commercial")]
        public record Preselected(string Type, string ProviderId, RemitterAccount? Remitter) : IDiscriminated;
    }
}
