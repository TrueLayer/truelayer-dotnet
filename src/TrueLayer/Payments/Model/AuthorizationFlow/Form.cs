using System.Collections.Generic;
using OneOf;

namespace TrueLayer.Payments.Model.AuthorizationFlow;

using InputTypesUnion = OneOf<InputTypes.Text, InputTypes.TextWithImage, InputTypes.Select>;

/// <summary>
/// Contains input type markers for form inputs.
/// </summary>
public static class InputTypes
{
    /// <summary>
    /// Represents a text input type.
    /// </summary>
    public record Text();

    /// <summary>
    /// Represents a text input with an associated image.
    /// </summary>
    public record TextWithImage();

    /// <summary>
    /// Represents a select/dropdown input type.
    /// </summary>
    public record Select();
}

/// <summary>
/// Represents a form containing various input types for user data collection.
/// </summary>
/// <param name="InputsTypes">The list of input types required by the form.</param>
public record Form(List<InputTypesUnion> InputsTypes);
