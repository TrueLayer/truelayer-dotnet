# TrueLayer SDK Acceptance Tests

The Acceptance Tests run against the TrueLayer Sandbox to validate the SDK against our APIs.

For the tests to run you need to set your Client ID and Secret. 

This can be done either using environment variables:

```
SDK_TrueLayer__ClientId=client_id SDK_TrueLayer__ClientSecret=client_secret dotnet test
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
