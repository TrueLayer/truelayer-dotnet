using System.Collections.Generic;
using TrueLayer.Models;

namespace TrueLayer.PaymentsProviders.Model;

public record AuthorizationFlow(AuthorizationFlowConfiguration Configuration);

public record AuthorizationFlowConfiguration(Dictionary<string, string>? Redirect = null, Form? Form = null,
    Consent? Consent = null);

public record Form(List<string> InputTypes);

public record Consent(ConsentRequirements Requirements);

public record ConsentRequirements(Dictionary<string, string> Pis, Dictionary<string, string>? Ais = null);
