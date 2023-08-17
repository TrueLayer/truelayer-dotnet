using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OneOf;
using static TrueLayer.Models.RedirectStatus;

namespace TrueLayer.Models
{
    /// <summary>
    /// Configuration object.
    /// </summary>
    /// <param name="ProviderSelection">Can the UI render a provider selection screen?</param>
    /// <param name="Redirect">Can the UI redirect the end user to a third-party page?</param>
    public record Configuration(ProviderSelection ProviderSelection, RedirectStatus Redirect);
}
