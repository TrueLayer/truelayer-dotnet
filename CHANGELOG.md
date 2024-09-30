# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

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
