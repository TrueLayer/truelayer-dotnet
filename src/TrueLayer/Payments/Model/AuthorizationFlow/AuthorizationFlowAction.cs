using System;
using System.Collections.Generic;
using OneOf;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model.AuthorizationFlow;

using AuthorizationFlowActionUnion = OneOf<
    Models.AuthorizationFlowAction.ProviderSelection,
    AuthorizationFlowAction.SchemeSelection,
    Models.AuthorizationFlowAction.Redirect,
    Models.AuthorizationFlowAction.WaitForOutcome,
    Models.AuthorizationFlowAction.Form,
    AuthorizationFlowAction.Consent,
    AuthorizationFlowAction.UserAccountSelection,
    AuthorizationFlowAction.Retry
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
/// <param name="Configuration">Optional configuration for the authorization flow.</param>
public record AuthorizationFlow(Actions Actions, Configuration? Configuration);

/// <summary>
/// This static class contains the different types of actions that can be taken during the authorization flow.
/// It contains only the actions/types that are for the payments flow and not already defined in the <see cref="TrueLayer.Models.AuthorizationFlowAction"/> class.
/// </summary>
public static class AuthorizationFlowAction
{
    /// <summary>
    /// Represents the hint for the subsequent action type in the authorization flow.
    /// </summary>
    public enum SubsequentActionHint
    {
        /// <summary>
        /// The next action will be a redirect.
        /// </summary>
        Redirect = 0,

        /// <summary>
        /// The next action will require form submission.
        /// </summary>
        Form = 1,

        /// <summary>
        /// The next action requires waiting for an outcome.
        /// </summary>
        Wait = 2
    };

    /// <summary>
    /// Represents a consent action in the authorization flow.
    /// </summary>
    /// <param name="Type">The type discriminator for this action.</param>
    /// <param name="Requirements">The consent requirements that need to be fulfilled.</param>
    /// <param name="SubsequentActionHint">Hint indicating the type of action that will follow.</param>
    [JsonDiscriminator("consent")]
    public record Consent(
        string Type,
        ConsentRequirements Requirements,
        SubsequentActionHint SubsequentActionHint) : IDiscriminated;

    /// <summary>
    /// Represents Account Information Service (AIS) consent requirements.
    /// </summary>
    /// <param name="RequiredScopes">List of required AIS consent scopes.</param>
    /// <param name="OptionalScopes">List of optional AIS consent scopes.</param>
    public record ConsentAisRequirement(
        List<ConsentAisScopes> RequiredScopes,
        List<ConsentAisScopes> OptionalScopes);

    /// <summary>
    /// Represents the complete set of consent requirements for both PIS and AIS.
    /// </summary>
    /// <param name="Pis">Payment Initiation Service consent requirements.</param>
    /// <param name="Ais">Account Information Service consent requirements.</param>
    public record ConsentRequirements(ConsentPisRequirement Pis, ConsentAisRequirement Ais);

    /// <summary>
    /// Represents an adjacent consent request.
    /// </summary>
    /// <param name="Requirements">The consent requirements for the adjacent consent.</param>
    public record AdjacentConsent(ConsentRequirements Requirements);

    /// <summary>
    /// Represents an adjacent action in the authorization flow.
    /// </summary>
    /// <param name="Consent">The consent action that is adjacent to the current action.</param>
    public record AdjacentAction(AdjacentConsent Consent);

    /// <summary>
    /// Represents a user account selection action where the user must select an account.
    /// </summary>
    /// <param name="Type">The type discriminator for this action.</param>
    /// <param name="Provider">The payment provider for which account selection is required.</param>
    /// <param name="MaskedAccountIdentifier">A masked version of the account identifier for security.</param>
    /// <param name="LastUsedAt">The timestamp when this account was last used.</param>
    [JsonDiscriminator("user_account_selection")]
    public record UserAccountSelection(
        string Type,
        Models.Provider Provider,
        string MaskedAccountIdentifier,
        DateTime LastUsedAt) : IDiscriminated;

    /// <summary>
    /// Represents a scheme selection action where the user must select a payment scheme.
    /// </summary>
    /// <param name="Type">The type discriminator for this action.</param>
    /// <param name="Schemes">The list of available payment schemes to choose from.</param>
    [JsonDiscriminator("scheme_selection")]
    public record SchemeSelection(
        string Type,
        List<Scheme> Schemes) : IDiscriminated;

    /// <summary>
    /// Represents a payment scheme with associated details.
    /// </summary>
    /// <param name="Id">The unique identifier for the scheme.</param>
    /// <param name="Recommended">Indicates whether this scheme is recommended.</param>
    /// <param name="Fee">The fee associated with this scheme.</param>
    public record Scheme(
        string Id,
        bool Recommended,
        Fee Fee);

    /// <summary>
    /// Represents a fee amount in a specific currency.
    /// </summary>
    /// <param name="AmountInMinor">The fee amount in minor currency units (e.g., cents for USD).</param>
    /// <param name="Currency">The currency code for the fee.</param>
    public record Fee(int AmountInMinor, string Currency);

    /// <summary>
    /// Represents a retry action that allows the user to retry the authorization flow.
    /// </summary>
    /// <param name="Type">The type discriminator for this action.</param>
    /// <param name="RetryOptions">The list of available retry options.</param>
    [JsonDiscriminator("retry")]
    public record Retry(string Type, List<RetryOption> RetryOptions) : IDiscriminated;

    /// <summary>
    /// Represents the available retry options for a failed authorization flow.
    /// </summary>
    public enum RetryOption
    {
        /// <summary>
        /// Restart the authorization flow from the beginning.
        /// </summary>
        Restart,
    }
}
