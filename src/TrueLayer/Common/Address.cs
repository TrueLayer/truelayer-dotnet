namespace TrueLayer.Common;

/// <summary>
/// Represents a physical address
/// </summary>
public record Address
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Address"/> class with the specified address components.
    /// </summary>
    /// <param name="city">The city or locality name</param>
    /// <param name="state">The state or county name</param>
    /// <param name="zip">The zip code or postal code</param>
    /// <param name="countryCode">The country code according to ISO-3166-1 alpha-2</param>
    /// <param name="addressLine1">Full street address including house number and street name</param>
    /// <param name="addressLine2">Optional details like building name, suite, apartment number</param>
    public Address(
        string city,
        string state,
        string zip,
        string countryCode,
        string addressLine1,
        string? addressLine2 = null)
    {
        AddressLine1 = addressLine1.NotNullOrWhiteSpace(nameof(addressLine1));
        AddressLine2 = addressLine2.NotEmptyOrWhiteSpace(nameof(addressLine2));
        City = city.NotNullOrWhiteSpace(nameof(city));
        State = state.NotNullOrWhiteSpace(nameof(state));
        Zip = zip.NotNullOrWhiteSpace(nameof(zip));
        CountryCode = countryCode.NotNullOrWhiteSpace(nameof(countryCode));
    }

    /// <summary>
    /// Gets full street address including house number and street name.
    /// </summary>
    public string AddressLine1 { get; }

    /// <summary>
    /// Gets details like building name, suite, apartment number, etc.
    /// </summary>
    public string? AddressLine2 { get; }

    /// <summary>
    /// Gets the name of the city / locality.
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Gets the name of the country / stat.
    /// </summary>
    public string State { get; }

    /// <summary>
    /// Gets the zip code or postal code
    /// </summary>
    public string Zip { get; }

    /// <summary>
    /// Gets the country code according to ISO-3166-1 alpha-2
    /// </summary>
    public string CountryCode { get; }
}
