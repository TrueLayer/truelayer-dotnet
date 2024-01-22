# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

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
