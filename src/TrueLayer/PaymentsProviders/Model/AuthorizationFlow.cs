using System.Collections.Generic;

namespace TrueLayer.PaymentsProviders.Model;

/// <summary>
/// Represents the authorization flow configuration for a payment provider.
/// </summary>
/// <param name="Configuration">The configuration details for the authorization flow.</param>
public record AuthorizationFlow(AuthorizationFlowConfiguration Configuration);

/// <summary>
/// Represents the configuration options for an authorization flow.
/// </summary>
/// <param name="Redirect">Optional redirect configuration parameters.</param>
/// <param name="Form">Optional form configuration for user input collection.</param>
/// <param name="Consent">Optional consent configuration requirements.</param>
public record AuthorizationFlowConfiguration(Dictionary<string, string>? Redirect = null, Form? Form = null,
    Consent? Consent = null);

/// <summary>
/// Represents a form configuration specifying required input types.
/// </summary>
/// <param name="InputTypes">The list of input types required by the form.</param>
public record Form(List<string> InputTypes);

/// <summary>
/// Represents consent requirements for the authorization flow.
/// </summary>
/// <param name="Requirements">The consent requirements that must be fulfilled.</param>
public record Consent(ConsentRequirements Requirements);

/// <summary>
/// Represents the consent requirements for Payment Initiation Service (PIS) and Account Information Service (AIS).
/// </summary>
/// <param name="Pis">Payment Initiation Service consent requirements.</param>
/// <param name="Ais">Optional Account Information Service consent requirements.</param>
public record ConsentRequirements(Dictionary<string, string> Pis, Dictionary<string, string>? Ais = null);
