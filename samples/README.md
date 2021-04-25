# Truelayer Sample Application
A very lightweight Razor Pages application to demonstrate how to use the TrueLayer's .NET Sdk.

## Running the application
Make sure to fill the `clientId` and `clientSecret` fields inside your `appsettings.json`.
You can obtain them by signing up at [TrueLayer's console](https://console.truelayer.com/?auto=signup):
```JSON
{
  "Truelayer": {
    "ClientId": "",
    "ClientSecret": "",
    "UseSandbox": ""
  }
}
```

Run the application using your favorite IDE or via command line from within the sample app folder with:
```
dotnet run
```

The application should be running on `http://localhost:5000` or `https://localhost:5001`.

There's a built-in SQLite database managed by Entity Framework Core to keep track of all the test payments you make.
To properly setup your db just run:
```
dotnet ef database update
```
from within the sample app folder and you should be ready to go.
