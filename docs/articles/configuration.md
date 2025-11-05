# Configuration

Advanced configuration options for the TrueLayer .NET client.

## Configuration Options

### TrueLayerOptions

- `ClientId` - Your TrueLayer client ID
- `ClientSecret` - Your TrueLayer client secret
- `UseSandbox` - Use sandbox environment (true) or production (false)
- `Auth` - Authentication API options
- `Payments` - Payments API options including signing key configuration

### Example

```csharp
services.AddTrueLayer(configuration, options =>
{
    options.UseSandbox = true;
    options.Auth = new ApiOptions
    {
        Uri = new Uri("https://custom-auth.truelayer.com")
    };
    if (options.Payments?.SigningKey != null)
    {
        options.Payments.SigningKey.PrivateKey = privateKey;
    }
});
```

## See Also

- [Authentication](authentication.md)
- [API Reference](xref:TrueLayer.TrueLayerOptions)
