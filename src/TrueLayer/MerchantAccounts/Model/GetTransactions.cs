using System;
using System.Collections.Generic;
using OneOf;
using TrueLayer.Models;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;
using PayoutBeneficiary = TrueLayer.Payouts.Model.GetPayoutBeneficiary;
using static TrueLayer.MerchantAccounts.Model.MerchantAccountTransactions;

namespace TrueLayer.MerchantAccounts.Model;

using ReturnForUnion = OneOf<ReturnFor.Identified, ReturnFor.Unknown>;
using MerchantAccountTransactionUnion = OneOf<MerchantAccountPayment, ExternalPayment, PendingPayout, ExecutedPayout, Refund>;
using PayoutBeneficiaryUnion = OneOf<
        PayoutBeneficiary.PaymentSource,
        PayoutBeneficiary.ExternalAccount,
        PayoutBeneficiary.BusinessAccount,
        PayoutBeneficiary.UserDetermined>;

/// <summary>
/// Represents the response from a request to retrieve merchant account transactions.
/// </summary>
/// <param name="Items">The list of transactions in the merchant account.</param>
/// <param name="Pagination">Pagination information for the transaction list.</param>
public record GetTransactionsResponse(IList<MerchantAccountTransactionUnion> Items, Pagination Pagination);

/// <summary>
/// Contains all transaction types and models for merchant account transactions.
/// </summary>
public static class MerchantAccountTransactions
{
    /// <summary>
    /// Defines the transaction type constants.
    /// </summary>
    public static class TransactionTypes
    {
        /// <summary>
        /// Represents a payment transaction type.
        /// </summary>
        public const string Payment = "payment";

        /// <summary>
        /// Represents a payout transaction type.
        /// </summary>
        public const string Payout = "payout";
    }

    /// <summary>
    /// Defines the base transaction model for merchant accounts
    /// </summary>
    /// <param name="Id">Unique ID for the Transaction</param>
    /// <param name="Currency">The payment's currency</param>
    /// <param name="AmountInMinor">Payment's amount in minor. It has to be >= 1</param>
    /// <param name="Status">Status of the payment (settled)</param>
    public abstract record BaseTransaction(
        string Id,
        string Currency,
        int AmountInMinor,
        string Status);

    /// <summary>
    /// Base class for payout transactions in the merchant account
    /// </summary>
    /// <param name="Id">Unique ID for the Transaction</param>
    /// <param name="Currency">The payment's currency</param>
    /// <param name="AmountInMinor">Payment's amount in minor. It has to be >= 1</param>
    /// <param name="Status">Status of the payment</param>
    /// <param name="CreatedAt">The date and time the transaction was created</param>
    /// <param name="Beneficiary">The transaction's payout beneficiary</param>
    /// <param name="ContextCode">The context code for the payout</param>
    /// <param name="PayoutId">Unique ID for the payout</param>
    public abstract record BaseTransactionPayout(
        string Id,
        string Currency,
        int AmountInMinor,
        string Status,
        DateTimeOffset CreatedAt,
        PayoutBeneficiaryUnion Beneficiary,
        string ContextCode,
        string PayoutId) : BaseTransaction(Id, Currency, AmountInMinor, Status);

    /// <summary>
    /// Represents a payment into the merchant account initiated by a TrueLayer Payment
    /// </summary>
    /// <param name="Id">Unique ID for the Transaction</param>
    /// <param name="Currency">The payment's currency</param>
    /// <param name="AmountInMinor">Payment's amount in minor. It has to be >= 1</param>
    /// <param name="Status">Status of the payment (settled)</param>
    /// <param name="SettledAt">The date and time the transaction was settled</param>
    /// <param name="PaymentSource">The source of the payment</param>
    /// <param name="PaymentId">Unique ID for the payment</param>
    [JsonDiscriminator(Discriminator)]
    public sealed record MerchantAccountPayment(
        string Id,
        string Currency,
        int AmountInMinor,
        string Status,
        DateTimeOffset SettledAt,
        PaymentSource PaymentSource,
        string PaymentId) : BaseTransaction(Id, Currency, AmountInMinor, Status), IDiscriminated
    {
        const string Discriminator = "merchant_account_payment";

        /// <summary>
        /// Gets the type discriminator for merchant account payment transactions.
        /// </summary>
        public string Type => Discriminator;
    }

    /// <summary>
    /// Represents a payment into the merchant account not initiated by a TrueLayer Payment
    /// </summary>
    /// <param name="Id">Unique ID for the Transaction</param>
    /// <param name="Currency">The payment's currency</param>
    /// <param name="AmountInMinor">Payment's amount in minor. It has to be >= 1</param>
    /// <param name="Status">Status of the payment (settled)</param>
    /// <param name="SettledAt">The date and time the transaction was settled</param>
    /// <param name="Remitter">Defines the remitter account</param>
    /// <param name="ReturnFor">Defines type of outbound transaction</param>
    [JsonDiscriminator(Discriminator)]
    public sealed record ExternalPayment(
        string Id,
        string Currency,
        int AmountInMinor,
        string Status,
        DateTimeOffset SettledAt,
        RemitterAccount Remitter,
        ReturnForUnion ReturnFor) : BaseTransaction(Id, Currency, AmountInMinor, Status), IDiscriminated
    {
        const string Discriminator = "external_payment";

        /// <summary>
        /// Gets the type discriminator for external payment transactions.
        /// </summary>
        public string Type => Discriminator;
    }

    /// <summary>
    /// Represents a pending payment out of the merchant account
    /// </summary>
    /// <inheritdoc cref="BaseTransactionPayout"/>
    [JsonDiscriminator(Discriminator)]
    public sealed record PendingPayout(
        string Id,
        string Currency,
        int AmountInMinor,
        string Status,
        DateTimeOffset CreatedAt,
        PayoutBeneficiaryUnion Beneficiary,
        string ContextCode,
        string PayoutId)
        : BaseTransactionPayout(
            Id,
            Currency,
            AmountInMinor,
            Status,
            CreatedAt,
            Beneficiary,
            ContextCode,
            PayoutId), IDiscriminated
    {
        const string Discriminator = "pending_payout";

        /// <summary>
        /// Gets the type discriminator for pending payout transactions.
        /// </summary>
        public string Type => Discriminator;
    }

    /// <summary>
    /// Represents an executed payment out of the merchant account
    /// </summary>
    /// <param name="Id">Unique ID for the Transaction</param>
    /// <param name="Currency">The payment's currency</param>
    /// <param name="AmountInMinor">Payment's amount in minor. It has to be >= 1</param>
    /// <param name="Status">Status of the payment</param>
    /// <param name="CreatedAt">The date and time the transaction was created</param>
    /// <param name="ExecutedAt">The date and time the transaction was executed</param>
    /// <param name="Beneficiary">The transaction's payout beneficiary</param>
    /// <param name="ContextCode">The context code for the payout</param>
    /// <param name="PayoutId">Unique ID for the payout</param>
    /// <param name="ReturnedBy">Unique ID for the external payment that returned this payout</param>
    /// <param name="SchemeId">The id of the scheme used to execute the payout</param>
    [JsonDiscriminator(Discriminator)]
    public sealed record ExecutedPayout(
        string Id,
        string Currency,
        int AmountInMinor,
        string Status,
        DateTimeOffset CreatedAt,
        DateTimeOffset ExecutedAt,
        PayoutBeneficiaryUnion Beneficiary,
        string ContextCode,
        string PayoutId,
        string ReturnedBy,
        string? SchemeId)
        : BaseTransactionPayout(
            Id,
            Currency,
            AmountInMinor,
            Status,
            CreatedAt,
            Beneficiary,
            ContextCode,
            PayoutId), IDiscriminated
    {
        const string Discriminator = "executed_payout";

        /// <summary>
        /// Gets the type discriminator for executed payout transactions.
        /// </summary>
        public string Type => Discriminator;
    }

    /// <summary>
    /// Represents payment refund out of the merchant account
    /// </summary>
    /// <param name="Id">Unique ID for the Transaction</param>
    /// <param name="Currency">The payment's currency</param>
    /// <param name="AmountInMinor">Payment's amount in minor. It has to be >= 1</param>
    /// <param name="Status">Status of the payment</param>
    /// <param name="CreatedAt">The date and time the transaction was created</param>
    /// <param name="ExecutedAt">The date and time the transaction was executed</param>
    /// <param name="Beneficiary">The transaction's payout beneficiary</param>
    /// <param name="ContextCode">The context code for the refund</param>
    /// <param name="RefundId">Unique ID for the refund</param>
    /// <param name="PaymentId">Unique ID for the payment</param>
    /// <param name="ReturnedBy">Unique ID for the external payment that returned this payout</param>
    /// <param name="SchemeId">The id of the scheme used to execute the payout</param>
    [JsonDiscriminator(Discriminator)]
    public sealed record Refund(
        string Id,
        string Currency,
        int AmountInMinor,
        string Status,
        DateTimeOffset CreatedAt,
        DateTimeOffset ExecutedAt,
        PayoutBeneficiaryUnion Beneficiary,
        string ContextCode,
        string RefundId,
        string PaymentId,
        string ReturnedBy,
        string? SchemeId)
        : BaseTransaction(
            Id,
            Currency,
            AmountInMinor,
            Status), IDiscriminated
    {
        const string Discriminator = "refund";

        /// <summary>
        /// Gets the type discriminator for refund transactions.
        /// </summary>
        public string Type => Discriminator;
    }
}
