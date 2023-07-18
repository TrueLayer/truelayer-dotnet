using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Models
{
    public record Provider(
        string? Id = null,
        string? DisplayName = null,
        string? IconUri = null,
        string? LogoUri = null,
        string? BgColor = null,
        ProviderAvailability? Availability = null,
        string? CountryCode = null,
        List<string>? SearchAliases = null);
}
