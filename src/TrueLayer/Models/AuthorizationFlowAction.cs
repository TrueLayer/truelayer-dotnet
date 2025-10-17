using System;
using System.Collections.Generic;
using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Models.Input;

namespace TrueLayer.Models;

using InputUnion = OneOf<Input.Text, Input.TextWithImage, Input.Select>;

public static class AuthorizationFlowAction
{
    /// <summary>
    /// Provider selection represents the PSU action of selecting a provider.
    /// </summary>
    /// <param name="Type">provider_selection</param>
    /// <param name="Providers">List of providers to be presented to the PSU.</param>
    [JsonDiscriminator("provider_selection")]
    public record ProviderSelection(string Type, List<Provider> Providers) : IDiscriminated;

    public enum SubsequentActionHint { Redirect = 0, Form = 1 };

    /// <summary>
    /// Consent action.
    /// </summary>
    /// <param name="Type">consent</param>
    /// <param name="SubsequentActionHint"></param>
    [JsonDiscriminator("consent")]
    public record Consent(string Type, SubsequentActionHint SubsequentActionHint) : IDiscriminated;

    /// <summary>
    /// Form action represents the PSU action of entering further details into the UI.
    /// </summary>
    /// <param name="Type">form</param>
    /// <param name="Inputs"></param>
    [JsonDiscriminator("form")]
    public record Form(string Type, List<InputUnion> Inputs) : IDiscriminated;

    /// <summary>
    /// Indication that there are currently no actions to perform. Clients must poll until the status changes, or the next action is available.
    /// </summary>
    /// <param name="Type">wait</param>
    /// <param name="DisplayMessage">An optional message to be displayed to the end user while they are waiting.</param>
    [JsonDiscriminator("wait")]
    public record WaitForOutcome(string Type, DisplayText? DisplayMessage) : IDiscriminated;

    /// <summary>
    /// Redirect action.
    /// </summary>
    /// <param name="Type">redirect</param>
    /// <param name="Uri">URL the end user must be redirected to.</param>
    [JsonDiscriminator("redirect")]
    public record Redirect(string Type, Uri Uri) : IDiscriminated;
}