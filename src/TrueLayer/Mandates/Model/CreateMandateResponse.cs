using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueLayer.Payments.Model;

namespace TrueLayer.Mandates.Model
{
    /// <summary>
    /// Details of the created mandate
    /// </summary>
    /// <param name="Id">Unique ID for the mandate.</param>
    /// <param name="ResourceToken">Token to use with /mandate endpoints. It can be safely shared with a front end channel.</param>
    /// <param name="User">Details on the user of the created mandate. If the user_id was not passed in the request body while creating the mandate, an ID will be generated and returned in this response.</param>
    public record CreateMandateResponse(string Id, string ResourceToken, PaymentUser User);
}
