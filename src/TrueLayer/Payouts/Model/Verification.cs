using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrueLayer.Serialization;

namespace TrueLayer.Payouts.Model
{
    /// <summary>
    /// Represents transaction search criteria for payout verification
    /// </summary>
    public record TransactionSearchCriteria
    {
        /// <summary>
        /// Creates a new <see cref="TransactionSearchCriteria"/>
        /// </summary>
        /// <param name="tokens">List of search tokens to match against transaction descriptions</param>
        /// <param name="amountInMinor">Transaction amount in minor currency unit</param>
        /// <param name="currency">Three-letter ISO currency code</param>
        /// <param name="createdAt">Transaction creation date</param>
        public TransactionSearchCriteria(
            IEnumerable<string> tokens,
            long amountInMinor,
            string currency,
            DateTime createdAt)
        {
            Tokens = tokens.NotNull(nameof(tokens));
            AmountInMinor = amountInMinor.GreaterThan(0, nameof(amountInMinor));
            Currency = currency.NotNullOrWhiteSpace(nameof(currency));
            CreatedAt = createdAt;
        }

        /// <summary>
        /// Gets the list of search tokens to match against transaction descriptions
        /// </summary>
        public IEnumerable<string> Tokens { get; }

        /// <summary>
        /// Gets the transaction amount in minor currency unit
        /// </summary>
        public long AmountInMinor { get; }

        /// <summary>
        /// Gets the three-letter ISO currency code
        /// </summary>
        public string Currency { get; }

        /// <summary>
        /// Gets the transaction creation date
        /// </summary>
        [JsonConverter(typeof(DateTimeDateOnlyJsonConverter))]
        public DateTime CreatedAt { get; }
    }

    /// <summary>
    /// Represents verification configuration for user-determined payouts
    /// </summary>
    public record Verification
    {
        /// <summary>
        /// Creates a new <see cref="Verification"/> with name verification only
        /// </summary>
        /// <param name="verifyName">Whether to verify the account holder name</param>
        public Verification(bool verifyName)
        {
            VerifyName = verifyName;
        }

        /// <summary>
        /// Creates a new <see cref="Verification"/> with name and transaction verification
        /// </summary>
        /// <param name="verifyName">Whether to verify the account holder name</param>
        /// <param name="transactionSearchCriteria">Transaction search criteria for verification</param>
        public Verification(bool verifyName, TransactionSearchCriteria transactionSearchCriteria)
        {
            VerifyName = verifyName;
            TransactionSearchCriteria = transactionSearchCriteria.NotNull(nameof(transactionSearchCriteria));
        }

        /// <summary>
        /// Gets whether to verify the account holder name
        /// </summary>
        public bool VerifyName { get; }

        /// <summary>
        /// Gets the transaction search criteria for verification
        /// </summary>
        public TransactionSearchCriteria? TransactionSearchCriteria { get; }
    }
}
