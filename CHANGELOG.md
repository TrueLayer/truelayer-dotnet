# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased]
### Changed
- Use [TrueLayer.Signing](https://www.nuget.org/packages/TrueLayer.Signing/0.1.11) nuget in favor of the custom-made `RequestSignature` class.
### Added
- `CHANGELOG` file.

## [0.2.2] - 2022-08-18
### Added
- `payment_source` payout beneficiary.
- `business_account` payout beneficiary.
- `executed` payment status.
### Removed
- `successful` payment status.
