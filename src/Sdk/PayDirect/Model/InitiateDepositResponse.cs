using System;

namespace TrueLayer.PayDirect.Model
{
    public record InitiateDepositResponse(InitiateDepositResponse.DepositResponseDetails Deposit, InitiateDepositResponse.AuthFlowResponseDetails AuthFlow)
    {
        public record DepositResponseDetails(DateTimeOffset InitiatedAt, string Status);

        public record AuthFlowResponseDetails(string Type, string Uri, DateTimeOffset? Expiry);
    }
}
