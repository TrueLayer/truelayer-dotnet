using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;

namespace TrueLayer.Models
{
    using InputUnion = OneOf<Input.Text, Input.TextWithImage, Input.Select>;

    internal record Redirect (Uri ReturnUri, Uri DirectReturnUri);

    internal record Consent();

    internal record Form(List<InputUnion> InputTypes);

    internal record StartAuthorizationFlowRequest(
        ProviderSelection ProviderSelection,
        Redirect Redirect,
        Consent Consent,
        Form Form);
}
