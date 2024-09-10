using System;
using System.Collections.Generic;
using OneOf;
using TrueLayer.Models;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;
using PayoutBeneficiary = TrueLayer.Payouts.Model.Beneficiary;
using static TrueLayer.MerchantAccounts.Model.MerchantAccountTransactions;

namespace TrueLayer.MerchantAccounts.Model;

using ReturnForUnion = OneOf<ReturnFor.Idenfied, ReturnFor.Unknow>;
using MerchantAccountTransactionUnion = OneOf<MerchantAccountPayment, ExternalPayment, PendingPayout, ExecutedPayout, Refund>;
using PayoutBeneficiaryUnion = OneOf<
        PayoutBeneficiary.PaymentSource,
        PayoutBeneficiary.ExternalAccount,
        PayoutBeneficiary.BusinessAccount,
        PayoutBeneficiary.UserDetermined>;

public record GetTransactionsResponse(IList<MerchantAccountTransactionUnion> Items, Pagination Pagination);

public static class MerchantAccountTransactions
{
    public static class TransactionTypes
    {
        public const string Payment = "payment";
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
    ///
    /// </summary>
    /// <inheritdoc cref="BaseTransaction"/>
    /// <param name="CreatedAt">The date and time the transaction was created</param>
    /// <param name="Beneficiary">The transaction's payout beneficiary</param>
    /// <param name="ContextCode"></param>
    /// <param name="PayoutId"></param>
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
    /// <inheritdoc cref="BaseTransaction"/>
    /// <param name="SettledAt">The date and time the transaction was settled</param>
    /// <param name="PaymentSource"></param>
    /// <param name="PaymentId"></param>
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
        public string Type => Discriminator;
    }

    /// <summary>
    /// Represents a payment into the merchant account not initiated by a TrueLayer Payment
    /// </summary>
    /// <inheritdoc cref="BaseTransaction"/>
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
        public string Type => Discriminator;
    }

    /// <summary>
    /// Represents an executed payment out of the merchant account
    /// </summary>
    /// <inheritdoc cref="BaseTransactionPayout"/>
    /// <param name="ExecutedAt">The date and time the transaction was executed</param>
    /// <param name="ReturnedBy">Unique ID for the external payment that returned this payout</param>
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
        string ReturnedBy)
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
        public string Type => Discriminator;
    }

    /// <summary>
    /// Represents payment refund out of the merchant account
    /// </summary>
    /// <inheritdoc cref="BaseTransaction"/>
    /// <param name="CreatedAt">The date and time the transaction was created</param>
    /// <param name="ExecutedAt">The date and time the transaction was executed</param>
    /// <param name="Beneficiary">The transaction's payout beneficiary</param>
    /// <param name="ContextCode">The context code for the refund</param>
    /// <param name="RefundId">Unique ID for the refund</param>
    /// <param name="PaymentId">Unique ID for the payment</param>
    /// <param name="ReturnedBy">Unique ID for the external payment that returned this payout</param>
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
        string ReturnedBy)
        : BaseTransaction(
            Id,
            Currency,
            AmountInMinor,
            Status), IDiscriminated
    {
        const string Discriminator = "refund";
        public string Type => Discriminator;
    }
}
