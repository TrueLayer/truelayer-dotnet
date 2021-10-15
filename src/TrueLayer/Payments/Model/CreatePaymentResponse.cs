using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueLayer.Payments.Model
{
    public record CreatePaymentResponse
    {

        public record AuthorizationRequired(string Id, long AmountInMinor, string Currency, DateTime CreatedAt, string Status, string ResourceToken)
            : CreatePaymentResponse
        {

        }
    }
}
