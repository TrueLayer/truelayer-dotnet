# TrueLayer SDK Acceptance Tests

The Acceptance Tests run against the TrueLayer Sandbox to validate the SDK against our APIs.

For the tests to run you need to set your Client ID and Secret. 

This can be done either using environment variables:

```
TrueLayer__ClientId=client_id TrueLayer__ClientSecret=client_secret dotnet test
```

Alternatively create an `appsettings.local.json` file in the root of the tests with the following content:

```json
{
  "TrueLayer": {
    "UseSandbox": true,
    "ClientId": "client_id",
    "ClientSecret": "client_secret"
  }
}
```

## Testing Payments

To test payments you will also need to provide a signing certificate. For convenience, a public/private key pair is provided in this directory (`ec512-private-key.pem` and `ec512-public-key.pem`) which should only be used for testing in Sandbox.

Upload the public certificate to the [TrueLayer Console](https://console.truelayer.com/) and then update your configuration with the generated key identifier:

```json
{
  "TrueLayer": {
    "UseSandbox": true,
    "ClientId": "client_id",
    "ClientSecret": "client_secret",
    "Payments": {
      "SigningKey": {
        "KeyId": "key_id"
      }
    }
  }
}
```

Or using an Environment Variable:

```
TrueLayer__PayDirect__SigningKey__KeyId=key_id dotnet test
```
