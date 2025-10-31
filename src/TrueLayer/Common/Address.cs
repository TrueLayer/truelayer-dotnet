namespace TrueLayer.Common;

public record Address
{
    /// <summary>
    /// Represents a physical address
    /// </summary>
    public Address(
        string city,
        string? state,
        string zip,
        string countryCode,
        string addressLine1,
        string? addressLine2 = null)
    {
        AddressLine1 = addressLine1.NotNullOrWhiteSpace(nameof(addressLine1));
        City = city.NotNullOrWhiteSpace(nameof(city));
        Zip = zip.NotNullOrWhiteSpace(nameof(zip));
        CountryCode = countryCode.NotNullOrWhiteSpace(nameof(countryCode));

        AddressLine2 = addressLine2.NotEmptyOrWhiteSpace(nameof(addressLine2));
        State = state.NotEmptyOrWhiteSpace(nameof(state));
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
    public string? State { get; }

    /// <summary>
    /// Gets the zip code or postal code
    /// </summary>
    public string Zip { get; }

    /// <summary>
    /// Gets the country code according to ISO-3166-1 alpha-2
    /// </summary>
    public string CountryCode { get; }
}
