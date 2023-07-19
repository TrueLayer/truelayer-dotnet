using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using static System.Net.Mime.MediaTypeNames;
using static TrueLayer.Mandates.Model.MandateDetail;

namespace TrueLayer.Mandates.Model
{
    using MandateDetailUnion = OneOf<AuthorizationRequiredMandateDetail, AuthorizingMandateDetail, AuthorizedMandateDetail, FailedMandateDetail, RevokedMandateDetail>;

    /// <summary>
    /// Mandates list.
    /// </summary>
    /// <param name="Items">Mandate detail items.</param>
    /// <param name="Pagination">Pagination object. Includes the next cursor to use as query parameters to fetch the page next to the one fetched.</param>
    public record ListMandatesResponse(List<MandateDetailUnion> Items, PaginationMetadata Pagination);
}
