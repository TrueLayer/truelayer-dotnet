using System;

namespace TrueLayer.PayDirect.Model
{
    /// <summary>
    /// Represents the response to a deposit request
    /// </summary>
    /// <param name="Deposit">The deposit details</param>
    /// <param name="AuthFlow">The authorization flow details</param>
    /// <returns></returns>
    public record DepositResponse(DepositResponse.DepositResponseDetails Deposit, DepositResponse.AuthFlowResponseDetails AuthFlow)
    {
        /// <summary>
        /// Details of the pending deposit
        /// </summary>
        /// <param name="DepositId">The deposit identifier</param>
        /// <param name="InitiatedAt">The date/time the deposit was initiated</param>
        /// <param name="Status">The status of the deposit</param>
        /// <returns></returns>
        public record DepositResponseDetails(Guid DepositId, DateTimeOffset InitiatedAt, string Status);

        public record AuthFlowResponseDetails(string Type, string Uri, DateTimeOffset? Expiry);
    }
}
