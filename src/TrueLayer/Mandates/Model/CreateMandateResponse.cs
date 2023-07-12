using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueLayer.Payments.Model;

namespace TrueLayer.Mandates.Model
{
    public record CreateMandateResponse(string id, string resourceToken, PaymentUser user);
}
