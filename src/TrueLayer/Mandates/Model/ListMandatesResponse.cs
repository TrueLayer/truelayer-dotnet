using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using static TrueLayer.Mandates.Model.MandateDetail;

namespace TrueLayer.Mandates.Model
{
    using MandateDetailUnion = OneOf<AuthorizationRequiredMandateDetail, AuthorizingMandateDetail, AuthorizedMandateDetail, FailedMandateDetail, RevokedMandateDetail>;

    public record ListMandatesResponse(List<MandateDetailUnion> items, PaginationMetadata pagination);
}
