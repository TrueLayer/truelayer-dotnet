using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;

namespace TrueLayer.Models
{
    using AuthorizationFlowActionUnion = OneOf<AuthorizationFlowAction.ProviderSelection, AuthorizationFlowAction.Consent, AuthorizationFlowAction.Form, AuthorizationFlowAction.WaitForOutcome, AuthorizationFlowAction.Redirect>;

    public record Actions(AuthorizationFlowActionUnion Next);

    public record AuthorizationFlow(Actions Actions);
}
