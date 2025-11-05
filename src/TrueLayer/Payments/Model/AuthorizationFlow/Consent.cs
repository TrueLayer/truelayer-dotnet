using System.Collections.Generic;


namespace TrueLayer.Payments.Model.AuthorizationFlow;

/// <summary>
/// Represents consent configuration for authorization flows.
/// </summary>
/// <param name="ActionType">The type of consent action to perform.</param>
/// <param name="Requirements">The consent requirements for PIS and AIS services.</param>
public record Consent(ConsentActionType? ActionType = null, ConsentRequirements? Requirements = null);

/// <summary>
/// Represents the complete set of consent requirements for both Payment Initiation Service and Account Information Service.
/// </summary>
/// <param name="Pis">Payment Initiation Service consent requirements.</param>
/// <param name="Ais">Account Information Service consent requirements.</param>
public record ConsentRequirements(ConsentPisRequirement Pis, ConsentAisRequirement Ais);

/// <summary>
/// Represents Payment Initiation Service (PIS) consent requirements.
/// </summary>
public record ConsentPisRequirement();

/// <summary>
/// Represents Account Information Service (AIS) consent requirements.
/// </summary>
/// <param name="Scopes">The list of AIS scopes being requested.</param>
public record ConsentAisRequirement(List<ConsentAisScopes> Scopes);

/// <summary>
/// Represents the available Account Information Service (AIS) consent scopes.
/// </summary>
public enum ConsentAisScopes
{
    /// <summary>
    /// Grants access to account information.
    /// </summary>
    Accounts = 0,

    /// <summary>
    /// Grants access to account balance information.
    /// </summary>
    Balance = 1,
}

/// <summary>
/// Represents the type of consent action in the authorization flow.
/// </summary>
public enum ConsentActionType
{
    /// <summary>
    /// Explicit consent requiring direct user action.
    /// </summary>
    Explicit,

    /// <summary>
    /// Adjacent consent that can be obtained alongside another action.
    /// </summary>
    Adjacent
}