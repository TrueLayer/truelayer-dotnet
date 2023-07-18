using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrueLayer.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ConfigurationStatus { Supported = 0, NotSupported = 1 };

    public record ProviderSelection(ConfigurationStatus Status);
}
