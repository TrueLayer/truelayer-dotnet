# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

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

## [1.16.0] - 2024-11-01

### Added

- Added support for `SchemeSelection` in Payouts

## [1.15.0] - 2024-10-15

### Added

- Added support for caching auth token

## [1.14.0] - 2024-10-01

### Added

- Added support for scheme selection override fields

## [1.13.0] - 2024-09-15

### Added

- Added support for multi Client support

## [1.12.0] - 2024-09-01

### Added

- Added support for generating idempotency keys when not provided

## [1.11.0] - 2024-08-15

### Added

- Added new payment features

## [1.10.0] - 2024-08-01

### Added

- Performance improvements

## [1.9.0] - 2024-07-15

### Added

- Additional API enhancements

## [1.8.0] - 2024-07-01

### Added

- API stability improvements

## [1.7.2] - 2024-06-15

### Fixed

- Bug fixes and improvements

## [1.7.0] - 2024-06-01

### Added

- New API features

## [1.6.1] - 2024-05-15

### Fixed

- Minor bug fixes

## [1.6.0] - 2024-05-01

### Added

- Initial stable release features

## [1.5.0] - 2024-04-15

### Added

- Core payment functionality

## [1.4.0] - 2024-04-01

### Added

- Basic API support

## [1.3.0] - 2024-03-15

### Added

- Foundation features

## [1.2.0] - 2024-03-01

### Added

- Early implementation

## [1.1.0] - 2024-02-15

### Added

- Beta features

## [1.0.0] - 2024-02-01

### Added

- Initial release
- Core TrueLayer API support
- Payment processing capabilities
- Authentication and authorization
- Basic merchant account management