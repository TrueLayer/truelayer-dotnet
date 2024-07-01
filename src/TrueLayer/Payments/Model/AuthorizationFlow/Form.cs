using System.Collections.Generic;
using OneOf;

namespace TrueLayer.Payments.Model.AuthorizationFlow;

using InputTypesUnion = OneOf<InputTypes.Text, InputTypes.TextWithImage, InputTypes.Select>;
public static class InputTypes
{
    public record Text();
    public record TextWithImage();
    public record Select();
}

public record Form(List<InputTypesUnion> InputsTypes);
