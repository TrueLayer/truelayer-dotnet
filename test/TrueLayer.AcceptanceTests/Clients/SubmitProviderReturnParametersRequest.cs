using System.Text.Json.Serialization;

namespace TrueLayer.AcceptanceTests.Clients;

public class SubmitProviderReturnParametersRequest
{
    [JsonPropertyName("query")] public string? Query { get; set; }
    [JsonPropertyName("fragment")] public string? Fragment { get; set; }
}
