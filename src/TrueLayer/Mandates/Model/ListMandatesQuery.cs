using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Mandates.Model
{
    public record ListMandatesQuery(string UserId, string Cursor, int Limit);
}
