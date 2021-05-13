using System;

namespace TrueLayer.PayDirect.Model
{
    public record DepositResponse(DepositResponse.DepositResponseDetails Deposit, DepositResponse.AuthFlowResponseDetails AuthFlow)
    {
        public record DepositResponseDetails(Guid DepositId, DateTimeOffset InitiatedAt, string Status);

        public record AuthFlowResponseDetails(string Type, string Uri, DateTimeOffset? Expiry);
    }
}
