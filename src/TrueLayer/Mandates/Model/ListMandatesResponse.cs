using System.Collections.Generic;
using OneOf;
using static TrueLayer.Mandates.Model.MandateDetail;

namespace TrueLayer.Mandates.Model
{
    using MandateDetailUnion = OneOf<AuthorizationRequiredMandateDetail, AuthorizingMandateDetail, AuthorizedMandateDetail, FailedMandateDetail, RevokedMandateDetail>;

    /// <summary>
    /// Mandates list.
    /// </summary>
    /// <param name="Items">Mandate detail items.</param>
    /// <param name="Pagination">Pagination object. Includes the next cursor to use as query parameters to fetch the page next to the one fetched.</param>
    internal record ListMandatesResponse(List<MandateDetailUnion> Items, PaginationMetadata Pagination);
}
