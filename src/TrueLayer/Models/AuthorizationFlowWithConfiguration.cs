using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Models
{
    public record AuthorizationFlowWithConfiguration
        (Actions Actions, Configuration Configuration): AuthorizationFlow
        (Actions);
}
