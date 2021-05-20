using System;

namespace TrueLayer.PayDirect.Model
{
    /// <summary>
    /// Represents a users account used as a source for deposits and destination for withdrawals
    /// </summary>
    /// <param name="AccountId">The account identifier</param>
    /// <param name="Name">The account holder name</param>
    /// <param name="Iban">The account IBAN used for deposits</param>
    /// <returns></returns>
    public record UserAcccount(Guid AccountId, string Name, string Iban);
}
