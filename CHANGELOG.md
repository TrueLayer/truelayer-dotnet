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