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
    using RedirectStatusUnion = OneOf<SupportedRedirectStatus, NotSupportedRedirectStatus>;

    public record Configuration (ProviderSelection ProviderSelection, RedirectStatusUnion Redirect);
}
