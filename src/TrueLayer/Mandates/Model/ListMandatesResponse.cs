using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Mandates.Model
{
    public record ListMandatesResponse(List<MandateDetail> items, PaginationMetadata pagination);
}
