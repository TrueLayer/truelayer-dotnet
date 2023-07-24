using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;

namespace TrueLayer.Models
{
    using AuthorizationFlowActionUnion = OneOf<AuthorizationFlowAction.ProviderSelection, AuthorizationFlowAction.Consent, AuthorizationFlowAction.Form, AuthorizationFlowAction.WaitForOutcome, AuthorizationFlowAction.Redirect>;

    /// <summary>
    /// Contains information regarding the next action to be taken in the authorization flow.
    /// </summary>
    /// <param name="Next">The next action that can be performed.</param>
    internal record Actions(AuthorizationFlowActionUnion Next);

    /// <summary>
    /// Contains information regarding the nature and the state of the authorization flow.
    /// </summary>
    /// <param name="Actions">Contains the next action to be taken in the authorization flow.</param>
    internal record AuthorizationFlow(Actions Actions);
}
