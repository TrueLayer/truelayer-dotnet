using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Mandates.Model
{
    /// <summary>
    /// Details of the fund confirmation
    /// </summary>
    /// <param name="Confirmed">Whether the funds are confirmed.</param>
    /// <param name="ConfirmedAt">The date and time the funds were confirmed at.</param>
    public record GetConfirmationOfFundsResponse(bool Confirmed, DateTime ConfirmedAt);
}
