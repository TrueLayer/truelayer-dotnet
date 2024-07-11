using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Models
{
    /// <summary>
    /// Contains information regarding the next action to be taken in the authorization flow.
    /// </summary>
    /// <param name="Actions">Contains the next action to be taken in the authorization flow.</param>
    /// <param name="Configuration"></param>
    public record AuthorizationFlowWithConfiguration(Actions Actions, Configuration? Configuration = null) : AuthorizationFlow(Actions);
}
