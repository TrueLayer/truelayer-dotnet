# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [1.1.0] - 2023-02-22
### Added
- `Address` and `DateofBirth` to `PaymentUserRequest`.

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
