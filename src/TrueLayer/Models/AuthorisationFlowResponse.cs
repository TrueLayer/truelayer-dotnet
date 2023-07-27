using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueLayer.Serialization;

namespace TrueLayer.Models
{
    internal static class AuthorisationFlowResponse
    {
        [JsonDiscriminator("authorizing")]
        internal record AuthorizationFlowAuthorizing(string Status, AuthorizationFlow AuthorizationFlow);

        [JsonDiscriminator("failed")]
        internal record AuthorizationFlowAuthorizationFailed(string Status, string FailureStage, string FailureReason);
    }
}
