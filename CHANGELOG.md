# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased]
### Added
- Added support for **Verified Payouts** (UK-only feature)
  - New `Verification` model with `VerifyName` and optional `TransactionSearchCriteria` for name and transaction verification
  - New `PayoutUserRequest` and `PayoutUserResponse` models for user details in verified payouts
  - Updated `CreatePayoutBeneficiary.UserDetermined` beneficiary type with verification support
  - New `PayoutHppLinkBuilder` helper for generating Hosted Payment Page verification links
  - New `CreatePayoutResponse.AuthorizationRequired` response type for verified payouts requiring authorization
  - Separate `GetPayoutBeneficiary` namespace for GET response types (distinct from CREATE request types)
  - Enhanced `OneOfJsonConverter` with `[DefaultJsonDiscriminator]` support for fallback deserialization
- Added support for **Hosted Page** in Create Payment (alternative to `CreateHostedPaymentPageLink`)
  - New `HostedPageRequest` model for configuring hosted payment page parameters (`ReturnUri`, `CountryCode`, `LanguageCode`, `MaxWaitForResult`)
  - New `HostedPageResponse` model containing the auto-constructed hosted page URI
  - Updated `CreatePaymentRequest` to accept optional `HostedPage` parameter to receive the hosted page URI directly in the response
  - Updated `CreatePaymentResponse.AuthorizationRequired` to include `HostedPage` property with the hosted page URI when requested
  - The existing `CreateHostedPaymentPageLink` method remains available for backward compatibility
### Removed
- Removed support for .NET 6.0
- Removed support for .NET Standard 2.1
- Removed IsExternalInit shim (no longer needed for .NET 8.0+)
### Changed
- **BREAKING**: `CreatePayout` now returns `OneOf<AuthorizationRequired, Created>` instead of `CreatePayoutResponse`
  - Consumers must use `.Match()` to handle both response types
  - Standard payouts return `Created` with just the payout ID
  - Verified payouts return `AuthorizationRequired` with ID, status, resource token, and user details
- **BREAKING**: `GetPayout` now returns `OneOf<AuthorizationRequired, Pending, Authorized, Executed, Failed>` instead of `OneOf<Pending, Authorized, Executed, Failed>`
  - Added `AuthorizationRequired` status for verified payouts awaiting user authorization
  - Consumers must update `.Match()` calls to handle the new `AuthorizationRequired` type
- **BREAKING**: Renamed `Beneficiary` to `CreatePayoutBeneficiary` for clarity
- **BREAKING**: Simplified `CreatePayoutBeneficiary.BusinessAccount` to only require `Reference` (removed account holder name and identifier)
- **BREAKING**: Split Provider types into `CreateProvider` (for requests) and `GetProvider` (for responses)
  - `CreateProvider.UserSelected` - Use for creating payments/payouts/mandates with user-selected provider
  - `CreateProvider.Preselected` - Use for creating payments/payouts/mandates with preselected provider
  - `GetProvider.UserSelected` - Returned in GET responses, includes `ProviderId` and `SchemeId` fields
  - `GetProvider.Preselected` - Returned in GET responses, includes `SchemeId` field
  - Migration: Replace `Provider.UserSelected` with `CreateProvider.UserSelected` or `GetProvider.UserSelected` as appropriate
  - Migration: Replace `Provider.Preselected(providerId, schemeId)` with `CreateProvider.Preselected(providerId, new SchemeSelection.Preselected { SchemeId = schemeId })`
- **BREAKING**: Split PaymentMethod types into `CreatePaymentMethod` (for requests) and `GetPaymentMethod` (for responses)
  - `CreatePaymentMethod.BankTransfer` - Use for creating payments
  - `CreatePaymentMethod.Mandate` - Use for creating payments with mandates
  - `GetPaymentMethod.BankTransfer` - Returned in GET payment responses, includes `SchemeId` field
  - `GetPaymentMethod.Mandate` - Returned in GET payment responses
  - Migration: Replace `PaymentMethod.BankTransfer` with `CreatePaymentMethod.BankTransfer` or `GetPaymentMethod.BankTransfer` as appropriate
- **BREAKING**: `CreateProvider.Preselected` constructor now requires `SchemeSelection` parameter (no longer accepts optional `schemeId` string)
  - Old: `new Provider.Preselected("provider-id", "scheme-id")` or `new Provider.Preselected("provider-id")`
  - New: `new CreateProvider.Preselected("provider-id", new SchemeSelection.Preselected { SchemeId = "scheme-id" })`
- Updated `CreatePayoutBeneficiary.PaymentSource` GET response to include `AccountHolderName` and `AccountIdentifiers`
- Updated `GetPayout` to return `GetPayoutBeneficiary` types with populated account details
- Updated to C# 12.0 language version
- Modernized code to use C# 11/12 features (ArgumentNullException.ThrowIfNull)
- Removed all conditional compilation directives (no longer needed for .NET 8.0+)

## [1.25.0] - 2025-10-14
### Added
- Added `SchemeId` field to `ExecutedPayout` and `Refund` transaction types in Merchant Account transactions endpoint response
- Add support for polish payouts and `SubMerchants` field

## [1.24.0] - 2025-01-24
### Added
- Enhanced RefundUnion to include all refund statuses: `RefundExecuted` and `RefundFailed` in addition to existing `RefundPending` and `RefundAuthorized`
- Updated `ListPaymentRefunds` and `GetPaymentRefund` methods to support returning refunds in all possible states

## [1.23.0] - 2025-01-15
### Added
- Added support for additional payment features

## [1.22.0] - 2024-12-20
### Added
- Added support for enhanced payment processing

## [1.21.0] - 2024-12-15
### Added
- Added support for improved API responses

## [1.20.0] - 2024-12-11
### Added
- Added support for `CreditableAt` in `GetPaymentResult`

## [1.19.0] - 2024-12-02
### Added
- Added support for `StatementReference` in `MerchantAccount`

## [1.18.0] - 2024-11-25
### Added
- Added support Add support for mandates in `HppLinkBuilder`

## [1.17.0] - 2024-11-25
### Added
- Added support for net9/net8

## [1.16.0] - 2024-11-07
### Added
- Added support to the `Metadata` field in the `GetPayoutResponse`

## [1.15.0] - 2024-10-10
### Added
- Added support to the `SchemeId` field in the `GetPayoutResponse`

## [1.14.0] - 2024-09-30
### Added
- Added support to `POST /v3/payments/{id}/actions/cancel` endpoint for the `Payments` API

## [1.13.0] - 2024-09-11
### Added
- Added support to the `Beneficiary` field in the `GetPayoutResponse`
- Added support to `GET v3/merchant-accounts/{id}/transactions` endpoint for the `MerchantAccount` API

## [1.12.0] - 2024-08-13
### Added
- Added support to `risk_assessment` parameter in `CreatePaymentRequest`
- Added support to `metadata` field to `CreatePaymentRequest` and `GetPaymentResponse` models
- Added support to `verification` field on `MerchantAccount` beneficiary type for `CreatePaymentRequest`

## [1.11.0] - 2024-08-05
### Added
- Added support to `retry` parameter in `CreatePaymentRequest` model
- Added support to `AttemptFailed` status in `CreatePaymentResponse` model
### Changed
- `Truelayer.Payments.SigningKey.PrivateKey` can now be set via configuration binding

## [1.10.0] - 2024-08-01
### Changed
- Replaced `ReleaseChannels` parameter from `SearchPaymentsProvideerRequest` with `ResleaseChannel` (single `string` value)
### Added
- Added support to `GET /v3/payments/{id}/refunds` endpoint.
- Added support to `GET /v3/payments/{payment_id}/refunds/{refund_id}` endpoint.
- Added support to `POST /v3/payments/{payment_id}/refunds` endpoint.

## [1.9.0] - 2024-07-22
### Changed
- Removed unnecessary `[Obsolete]` attributes

## [1.8.0] - 2024-07-12
### Added
- Added support to `POST /v3/payments/{id}/authorization-flow` endpoint.
- Added support to `authorizathion_flow` parameter in `CreatePaymentRequest` model.

## [1.7.2] - 2024-06-18
### Added
- Added `POST /v3/payments-providers/search` endpoint.
- Fixed deserialization issue with some responses from the Mandates APIs

## [1.6.1] - 2024-02-13
### Changed
- Changed `GET payments-providers/{id}` to set the `Authorization` header on the request instead of the `client_id` on the query parameter.

## [1.6.0] - 2024-01-22
- Fixed SSRF vulnerability with a CVSS score of 8.6 (High)

## [1.5.0] - 2023-11-16
### Added
- Added scheme selection options for the provider selection objects to be submitted when creating a payment.

## [1.4.0] - 2023-10-17
### Added
- Added mandates APIs. Thanks to @mohammedmiah99, @Ryan-Palmer and @ubunoir for their contributions.

## [1.3.2] - 2023-10-09
### Added
- Added `RelatedProducts` to `CreatePaymentRequest`

## [1.3.1] - 2023-08-30
### Changed
- Set `TL-Agent` header instead of `user-agent`.

## [1.3.0] - 2023-07-19
### Added
- `Metadata` to `CreatePayoutRequest`.
- `Address` and `DateOfBirth` to `CreatePayoutRequest.Beneficiary.ExternalAccount`.

## [1.2.1] - 2023-04-26
### Fixed
- Updated payouts to use the `payments` token instead of the legacy `paydirect` token.

## [1.2.0] - 2023-03-06
### Added
- `Address` and `DateofBirth` to `PaymentUserRequest`.

## [1.1.0] - 2023-02-22
### Unlisted
- ~~`Address` and `DateofBirth` to `PaymentUserRequest`.~~
- This version got unlisted because wrong. Please refer to version `1.2.0`.

## [1.0.0] - 2023-01-23
### Changed
- `ProviderFilters.CustomerSegments` is now an array.

## [0.3.3] - 2023-01-10
### Removed
- Some user information from payment response (see [TrueLayer Changelog](https://docs.truelayer.com/changelog/removal-of-user-info-in-payment-and-mandate-response)).
    * `name`
    * `email`
    * `phone`
    * `date_of_birth`
    * `address`

## [0.3.0] - 2022-10-31
### Changed
- Upgrade to `.NET6`.

### Added
- `Status` to `CreatePaymentResponse` model.

## [0.2.3] - 2022-10-05
### Changed
- Use [TrueLayer.Signing](https://www.nuget.org/packages/TrueLayer.Signing/0.1.11) nuget in favor of the custom-made `RequestSignature` class.
### Added
- `CHANGELOG` file.
- `GET /payments-providers` endpoint.

## [0.2.2] - 2022-08-18
### Added
- `payment_source` payout beneficiary.
- `business_account` payout beneficiary.
- `executed` payment status.
### Removed
- `successful` payment status.
