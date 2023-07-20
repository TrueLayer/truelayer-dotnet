using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrueLayer.Models
{
    /// <summary>
    /// A provider to be presented to the PSU.
    /// </summary>
    /// <param name="Id">Unique ID for the Provider.</param>
    /// <param name="DisplayName"></param>
    /// <param name="IconUri"></param>
    /// <param name="LogoUri"></param>
    /// <param name="BgColor">pattern: ^#[A-F0-9]{6}$</param>
    /// <param name="Availability">Provider availability.</param>
    /// <param name="CountryCode"></param>
    /// <param name="SearchAliases">Alternative search terms that should be used to help users find this provider.</param>
    public record Provider(
        string? Id = null,
        string? DisplayName = null,
        string? IconUri = null,
        string? LogoUri = null,
        string? BgColor = null,
        ProviderAvailability? Availability = null,
        CountryCode? CountryCode = null,
        List<string>? SearchAliases = null);
}
