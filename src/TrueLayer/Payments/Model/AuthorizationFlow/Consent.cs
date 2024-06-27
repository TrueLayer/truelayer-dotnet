using System;
using System.Collections.Generic;
using OneOf;


namespace TrueLayer.Payments.Model.AuthorizationFlow
{
    using ConsentAisScopesUnion = OneOf<ConsentAisScopes.Accounts, ConsentAisScopes.Balance>;

    public record Consent(ConsentActionType? ActionType = null, ConsentRequirements? Requirements = null);

    public record ConsentRequirements(ConsentPisRequirement Pis, ConsentAisRequirement Ais);
    public record ConsentPisRequirement();
    public record ConsentAisRequirement(List<ConsentAisScopesUnion> Scopes);

    public static class ConsentAisScopes
    {
        public record Accounts();
        public record Balance();
    }

    public enum ConsentActionType
    {
        Explicit,
        Adjacent
    }
}
