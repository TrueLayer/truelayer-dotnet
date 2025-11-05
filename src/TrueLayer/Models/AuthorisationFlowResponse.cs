using TrueLayer.Serialization;

namespace TrueLayer.Models;

public static class AuthorisationFlowResponse
{
    /// <summary>
    /// Mandate Authorization Flow
    /// </summary>
    /// <param name="Status">authorizing</param>
    /// <param name="AuthorizationFlow">Contains information regarding the nature and the state of the authorization flow.</param>
    [JsonDiscriminator("authorizing")]
    public record AuthorizationFlowAuthorizing(string Status, AuthorizationFlow AuthorizationFlow);

    /// <summary>
    /// Mandate Authorization Flow
    /// </summary>
    /// <param name="Status">failed</param>
    /// <param name="FailureStage">The status the mandate was in when it failed.</param>
    /// <param name="FailureReason">A readable detail for why the mandate failed.</param>
    [JsonDiscriminator("failed")]
    public record AuthorizationFlowAuthorizationFailed(string Status, string FailureStage, string FailureReason);
}