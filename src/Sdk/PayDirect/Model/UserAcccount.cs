using System;

namespace TrueLayer.PayDirect.Model
{
    public record UserAcccount(Guid AccountId, string Name, string Iban);
}
