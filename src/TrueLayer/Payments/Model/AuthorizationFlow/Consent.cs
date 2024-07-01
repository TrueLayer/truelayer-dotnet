using System.Collections.Generic;


namespace TrueLayer.Payments.Model.AuthorizationFlow
{
    public record Consent(ConsentActionType? ActionType = null, ConsentRequirements? Requirements = null);

    public record ConsentRequirements(ConsentPisRequirement Pis, ConsentAisRequirement Ais);
    public record ConsentPisRequirement();
    public record ConsentAisRequirement(List<ConsentAisScopes> Scopes);

    public enum ConsentAisScopes
    {
        Accounts = 0,
        Balance = 1,
    }

    public enum ConsentActionType
    {
        Explicit,
        Adjacent
    }
}
