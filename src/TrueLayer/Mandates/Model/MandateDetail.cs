using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;
using static TrueLayer.Mandates.Model.Provider;
using static TrueLayer.Mandates.Model.Beneficiary;
using TrueLayer.Models;
using static TrueLayer.Mandates.Model.MandateDetail;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TrueLayer.Mandates.Model
{
    using ProviderUnion = OneOf<Payments.Model.Provider.UserSelected, Preselected>;
    using BeneficiaryUnion = OneOf<ExternalAccount, MerchantAccount>;
    using MandateDetailUnion = OneOf<AuthorizationRequiredMandateDetail, AuthorizingMandateDetail, AuthorizedMandateDetail, FailedMandateDetail, RevokedMandateDetail>;

    public static class MandateDetail
    {
        public abstract record MandateDetailBase(
            string Id,
            string Currency,
            BeneficiaryUnion Beneficiary,
            string Reference,
            DateTime CreatedAt,
            Constraints Constraints,
            ProviderUnion ProviderSelection,
            string Status,
            PaymentUser? User = null,
            Dictionary<string, string>? Metadata = null);

        /// <summary>
        /// A mandate yet to be authorized. This is the first status for a mandate that requires further actions to be authorized.
        /// </summary>
        /// <param name="Id">Unique ID for the mandate.</param>
        /// <param name="Currency">Three-letter ISO currency code.</param>
        /// <param name="Beneficiary">Represents a beneficiary account.</param>
        /// <param name="Reference">Reference set on the mandate.</param>
        /// <param name="CreatedAt">The date and time the mandate was created at.</param>
        /// <param name="Constraints">Sets the limits for the payments that can be created by the mandate. If a payment is attempted that doesn't fit within these constraints it will fail.</param>
        /// <param name="ProviderSelection">Provider selection.</param>
        /// <param name="Status">authorization_required.</param>
        /// <param name="User">User object.</param>
        /// <param name="Metadata">Optional field for adding custom key-value data to a resource. This object can contain a maximum of 10 key-value pairs, each with a key with a maximum length of 40 characters and a non-null value with a maximum length of 500 characters.</param>
        [JsonDiscriminator("authorization_required")]
        public record AuthorizationRequiredMandateDetail(
            string Id,
            string Currency,
            BeneficiaryUnion Beneficiary,
            string Reference,
            DateTime CreatedAt,
            Constraints Constraints,
            ProviderUnion ProviderSelection,
            string Status,
            PaymentUser? User = null,
            Dictionary<string, string>? Metadata = null)
            : MandateDetailBase(
                Id,
                Currency,
                Beneficiary,
                Reference,
                CreatedAt,
                Constraints,
                ProviderSelection,
                Status,
                User,
                Metadata);

        /// <summary>
        /// A mandate that is being authorized. This is the second status for a mandate.
        /// </summary>
        /// <param name="Id">Unique ID for the mandate.</param>
        /// <param name="Currency">Three-letter ISO currency code.</param>
        /// <param name="Beneficiary">Represents a beneficiary account.</param>
        /// <param name="Reference">Reference set on the mandate.</param>
        /// <param name="CreatedAt">The date and time the mandate was created at.</param>
        /// <param name="Constraints">Sets the limits for the payments that can be created by the mandate. If a payment is attempted that doesn't fit within these constraints it will fail.</param>
        /// <param name="ProviderSelection">Provider selection.</param>
        /// <param name="Status">authorizing</param>
        /// <param name="AuthorizationFlow">Contains information regarding the nature and the state of the authorization flow.</param>
        /// <param name="User">User object.</param>
        /// <param name="Metadata">Optional field for adding custom key-value data to a resource. This object can contain a maximum of 10 key-value pairs, each with a key with a maximum length of 40 characters and a non-null value with a maximum length of 500 characters.</param>
        [JsonDiscriminator("authorizing")]
        public record AuthorizingMandateDetail(
            string Id,
            string Currency,
            BeneficiaryUnion Beneficiary,
            string Reference,
            DateTime CreatedAt,
            Constraints Constraints,
            ProviderUnion ProviderSelection,
            string Status,
            AuthorizationFlowWithConfiguration AuthorizationFlow,
            PaymentUser? User,
            Dictionary<string, string>? Metadata)
            : MandateDetailBase(
                Id,
                Currency,
                Beneficiary,
                Reference,
                CreatedAt,
                Constraints,
                ProviderSelection,
                Status,
                User,
                Metadata);

        /// <summary>
        /// A mandate that has been authorized. This is the third status for a mandate.
        /// </summary>
        /// <param name="Id">Unique ID for the mandate.</param>
        /// <param name="Currency">Three-letter ISO currency code.</param>
        /// <param name="Beneficiary">Represents a beneficiary account.</param>
        /// <param name="Reference">Reference set on the mandate.</param>
        /// <param name="CreatedAt">The date and time the mandate was created at.</param>
        /// <param name="Constraints">Sets the limits for the payments that can be created by the mandate. If a payment is attempted that doesn't fit within these constraints it will fail.</param>
        /// <param name="ProviderSelection">Provider selection.</param>
        /// <param name="Status">authorized</param>
        /// <param name="AuthorizationFlow">Contains information regarding the nature and the state of the authorization flow.</param>
        /// <param name="AuthorizedAt">The date and time the mandate was authorized at.</param>
        /// <param name="Remitter">The routing information for the remitter bank account.</param>
        /// <param name="User">User object.</param>
        /// <param name="Metadata">Optional field for adding custom key-value data to a resource. This object can contain a maximum of 10 key-value pairs, each with a key with a maximum length of 40 characters and a non-null value with a maximum length of 500 characters.</param>
        [JsonDiscriminator("authorized")]
        public record AuthorizedMandateDetail(
            string Id,
            string Currency,
            BeneficiaryUnion Beneficiary,
            string Reference,
            DateTime CreatedAt,
            Constraints Constraints,
            ProviderUnion ProviderSelection,
            string Status,
            AuthorizationFlowWithConfiguration AuthorizationFlow,
            DateTime? AuthorizedAt = null,
            RemitterAccount? Remitter = null,
            PaymentUser? User = null,
            Dictionary<string, string>? Metadata = null)
            : MandateDetailBase(
                Id,
                Currency,
                Beneficiary,
                Reference,
                CreatedAt,
                Constraints,
                ProviderSelection,
                Status,
                User,
                Metadata);

        /// <summary>
        /// A mandate that has failed.
        /// </summary>
        /// <param name="Id">Unique ID for the mandate.</param>
        /// <param name="Currency">Three-letter ISO currency code.</param>
        /// <param name="Beneficiary">Represents a beneficiary account.</param>
        /// <param name="Reference">Reference set on the mandate.</param>
        /// <param name="CreatedAt">The date and time the mandate was created at.</param>
        /// <param name="Constraints">Sets the limits for the payments that can be created by the mandate. If a payment is attempted that doesn't fit within these constraints it will fail.</param>
        /// <param name="ProviderSelection">Provider selection.</param>
        /// <param name="FailureStage">The status the mandate was in when it failed.</param>
        /// <param name="FailureReason">A readable detail for why the mandate failed.</param>
        /// <param name="FailedAt">The date and time the mandate authorization was failed at.</param>
        /// <param name="Status">failed</param>
        /// <param name="AuthorizationFlow">Contains information regarding the nature and the state of the authorization flow.</param>
        /// <param name="User">User object.</param>
        /// <param name="Metadata">Optional field for adding custom key-value data to a resource. This object can contain a maximum of 10 key-value pairs, each with a key with a maximum length of 40 characters and a non-null value with a maximum length of 500 characters.</param>
        [JsonDiscriminator("failed")]
        public record FailedMandateDetail(
            string Id,
            string Currency,
            BeneficiaryUnion Beneficiary,
            string Reference,
            DateTime CreatedAt,
            Constraints Constraints,
            ProviderUnion ProviderSelection,
            string FailureStage,
            string FailureReason,
            DateTime FailedAt,
            string Status,
            AuthorizationFlowWithConfiguration AuthorizationFlow,
            PaymentUser? User = null,
            Dictionary<string, string>? Metadata = null)
            : MandateDetailBase(
                Id,
                Currency,
                Beneficiary,
                Reference,
                CreatedAt,
                Constraints,
                ProviderSelection,
                Status,
                User,
                Metadata);


        /// <summary>
        /// A mandate that has been revoked.
        /// </summary>
        /// <param name="Id">Unique ID for the mandate.</param>
        /// <param name="Currency">Three-letter ISO currency code.</param>
        /// <param name="Beneficiary">Represents a beneficiary account.</param>
        /// <param name="Reference">Reference set on the mandate.</param>
        /// <param name="CreatedAt">The date and time the mandate was created at.</param>
        /// <param name="Constraints">Sets the limits for the payments that can be created by the mandate. If a payment is attempted that doesn't fit within these constraints it will fail.</param>
        /// <param name="ProviderSelection">Provider selection.</param>
        /// <param name="RevocationSource">Source for the revocation.</param>
        /// <param name="AuthorizedAt">The date and time the mandate was authorized at.</param>
        /// <param name="RevokedAt">The date and time the mandate was revoked at.</param>
        /// <param name="Status">revoked</param>
        /// <param name="AuthorizationFlow">Contains information regarding the nature and the state of the authorization flow.</param>
        /// <param name="User">User object.</param>
        /// <param name="Metadata">Optional field for adding custom key-value data to a resource. This object can contain a maximum of 10 key-value pairs, each with a key with a maximum length of 40 characters and a non-null value with a maximum length of 500 characters.</param>
        [JsonDiscriminator("revoked")]
        public record RevokedMandateDetail(
            string Id,
            string Currency,
            BeneficiaryUnion Beneficiary,
            string Reference,
            DateTime CreatedAt,
            Constraints Constraints,
            ProviderUnion ProviderSelection,
            string RevocationSource,
            DateTime AuthorizedAt,
            DateTime RevokedAt,
            string Status,
            AuthorizationFlowWithConfiguration AuthorizationFlow,
            PaymentUser? User = null,
            Dictionary<string, string>? Metadata = null)
            : MandateDetailBase(
                Id,
                Currency,
                Beneficiary,
                Reference,
                CreatedAt,
                Constraints,
                ProviderSelection,
                Status,
                User,
                Metadata);
    }
}
