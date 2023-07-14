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
        /// <summary>
        /// Provider Selection
        /// </summary>
        /// <param name="Type">The type of provider.</param>
        /// <param name="ProviderId">The provider Id the PSU will use for this payment.</param>
        /// <param name="Remitter">Remitter</param>
        [JsonDiscriminator("commercial")]
        public record Preselected(string Type, string ProviderId, RemitterAccount? Remitter) : IDiscriminated;
    }
}
