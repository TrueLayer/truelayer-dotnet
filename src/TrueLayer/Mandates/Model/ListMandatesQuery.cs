using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Mandates.Model
{
    /// <summary>
    /// List all the mandates associated to the client used.
    /// </summary>
    /// <param name="UserId">ID of the user</param>
    /// <param name="Cursor">Cursor used for pagination purposes that represents the first item of the page. Returned as next_cursor in the response payload of endpoints supporting pagination. Not required to access the first page of items.</param>
    /// <param name="Limit">Maximum number of items included in the returned window. Should be greater than 0 and less than 50. If not set, a default of 25 is considered.</param>
    public record ListMandatesQuery(string UserId, string? Cursor = null, int? Limit = null);
}
