This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Common Development Commands

### Build and Test
- **Build the solution**: `dotnet build`
- **Run unit tests**: `dotnet test test/TrueLayer.Tests/TrueLayer.Tests.csproj`
- **Run acceptance tests**: `dotnet test test/TrueLayer.AcceptanceTests/TrueLayer.AcceptanceTests.csproj`
- **Run specific test**: `dotnet test --filter "TestMethodName"`
- **Generate coverage reports**: Coverage is generated automatically during the build process
### Package Management
- **Pack NuGet packages**: `dotnet cake --target=Pack`
- **Clean artifacts**: `dotnet cake --target=Clean`
### Project Structure Commands
- **Restore tools**: `dotnet tool restore`
- **Build specific project**: `dotnet build src/TrueLayer/TrueLayer.csproj`
## Architecture Overview
### Core Client Architecture
The TrueLayer .NET client follows a modular architecture with these main components:
- **ITrueLayerClient**: Main interface providing access to all API modules
- **TrueLayerClient**: Concrete implementation using lazy initialization for API modules
- **ApiClient**: HTTP client wrapper handling authentication and request signing
- **Authentication**: JWT token-based auth with optional caching (InMemory/Custom)
### API Modules
Each API area is encapsulated in its own module:
- **Auth**: Token management (`IAuthApi`)
- **Payments**: Payment creation and management (`IPaymentsApi`)
- **Payouts**: Payout operations (`IPayoutsApi`)
- **PaymentsProviders**: Provider discovery (`IPaymentsProvidersApi`)
- **MerchantAccounts**: Account management (`IMerchantAccountsApi`)
- **Mandates**: Mandate operations (`IMandatesApi`)
### Key Architectural Patterns
- **Dependency Injection**: Full DI support with `AddTrueLayer()` and `AddKeyedTrueLayer()` extensions
- **Options Pattern**: Configuration via `TrueLayerOptions` with support for multiple clients
- **Authentication Caching**: Configurable auth token caching strategies
- **Request Signing**: Cryptographic signing using EC512 keys via TrueLayer.Signing package
- **Polymorphic Serialization**: OneOf types for discriminated unions, custom JSON converters
### Target Frameworks
- .NET 10.0
- .NET 9.0
- .NET 8.0
### Testing Structure
- **Unit Tests**: `/test/TrueLayer.Tests/` - Fast, isolated tests with mocking
- **Acceptance Tests**: `/test/TrueLayer.AcceptanceTests/` - Integration tests against real/mock APIs
- **Benchmarks**: `/test/TrueLayer.Benchmarks/` - Performance testing
### Key Dependencies
- **OneOf**: Discriminated union types for polymorphic models
- **TrueLayer.Signing**: Request signing functionality
- **Microsoft.Extensions.*****: Standard .NET extensions for DI, HTTP, caching, configuration
- **System.Text.Json**: Primary serialization with custom converters
### Configuration Requirements
- ClientId, ClientSecret for API authentication
- SigningKey (KeyId + PrivateKey) for payment request signing
- UseSandbox flag for environment selection
- Optional auth token caching configuration
### Build System
Uses Cake build system (`build.cake`) with tasks for:
- Clean, Build, Test, Pack, GenerateReports
- Coverage reporting via Coverlet
- NuGet package publishing
- CI/CD integration with GitHub Actions
### Code Style
- C# 12.0 language features
- Nullable reference types enabled
- Code style enforcement via `EnforceCodeStyleInBuild`
- EditorConfig and analyzer rules applied

## Pull Request Guidelines
When creating a PR, Claude will ask for a JIRA ticket reference if:
- The GitHub user is part of the Api Client Libraries team at TrueLayer
- The GitHub username has a `tl-` prefix
- When in doubt, an optional ACL ticket reference will be requested

Format: `[ACL-XXX]` in the PR title for JIRA ticket references.
