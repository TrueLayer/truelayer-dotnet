using System;
using System.Collections.Generic;
using TrueLayer.Serialization;
using OneOf;

namespace TrueLayer.Payments.Model.AuthorizationFlow;

using AuthorizationFlowActionUnion = OneOf<
    Models.AuthorizationFlowAction.ProviderSelection,
    AuthorizationFlowAction.SchemeSelection,
    Models.AuthorizationFlowAction.Redirect,
    Models.AuthorizationFlowAction.WaitForOutcome,
    Models.AuthorizationFlowAction.Form,
    AuthorizationFlowAction.Consent,
    AuthorizationFlowAction.UserAccountSelection
>;

/// <summary>
/// Contains information regarding the next action to be taken in the authorization flow.
/// </summary>
/// <param name="Next">The next action that can be performed.</param>
public record Actions(AuthorizationFlowActionUnion Next);

/// <summary>
/// Contains information regarding the nature and the state of the authorization flow.
/// </summary>
/// <param name="Actions">Contains the next action to be taken in the authorization flow.</param>
public record AuthorizationFlow(Actions Actions);

/// <summary>
/// This static class contains the different types of actions that can be taken during the authorization flow.
/// It contains only the actions/types that are for the payments flow and not already defined in the <see cref="TrueLayer.Models.AuthorizationFlowAction"/> class.
/// </summary>
public static class AuthorizationFlowAction
{
    public enum SubsequentActionHint { Redirect = 0, Form = 1, Wait = 2 };

    [JsonDiscriminator("consent")]
    public record Consent(
        string Type,
        ConsentRequirements Requirements,
        SubsequentActionHint SubsequentActionHint) : IDiscriminated;

    public record ConsentAisRequirement(
        List<ConsentAisScopes> RequiredScopes,
        List<ConsentAisScopes> OptionalScopes);

    public record ConsentRequirements(ConsentPisRequirement Pis, ConsentAisRequirement Ais);

    public record AdjacentConsent(ConsentRequirements Requirements);

    public record AdjacentAction(AdjacentConsent Consent);

    [JsonDiscriminator("user_account_selection")]
    public record UserAccountSelection(
        string Type,
        Models.Provider Provider,
        string MaskedAccountIdentifier,
        DateTime LastUsedAt) : IDiscriminated;

    [JsonDiscriminator("scheme_selection")]
    public record SchemeSelection(
        string Type,
        List<Scheme> Schemes) : IDiscriminated;

    public record Scheme(
        string Id,
        bool Recommended,
        Fee Fee);

    public record Fee(int AmountInMinor, string Currency);
}
