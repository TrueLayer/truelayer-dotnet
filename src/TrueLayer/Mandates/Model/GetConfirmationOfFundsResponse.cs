using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Mandates.Model
{
    public record GetConfirmationOfFundsResponse(bool Confirmed, string Confirmed_at);
}
