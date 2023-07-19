using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrueLayer.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CountryCode { AT = 1, BE = 2, DE = 3, DK = 4, ES = 5, FI = 6, FR = 7, GB = 8, IE = 9, IT = 10, LT = 11, NL = 12, NO = 13, PL = 14, PT = 15, RO = 16 }

}
