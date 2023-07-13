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

namespace TrueLayer.Mandates.Model
{
    using ProviderUnion = OneOf<Payments.Model.Provider.UserSelected, Preselected>;
    using BeneficiaryUnion = OneOf<ExternalAccount, MerchantAccount>;

    public static class MandateDetail
    {
        public abstract record MandateDetailBase(
            string Id,
            string Currency,
            BeneficiaryUnion Beneficiary,
            string Reference,
            PaymentUser User,
            DateTime CreatedAt,
            Constraints Constraints,
            Dictionary<string, string> Metadata,
            ProviderUnion ProviderSelection,
            string Status);

        [JsonDiscriminator(Discriminator)]
        public record AuthorizationRequiredMandateDetail(
            string Id,
            string Currency,
            BeneficiaryUnion Beneficiary,
            string Reference,
            PaymentUser User,
            DateTime CreatedAt,
            Constraints Constraints,
            Dictionary<string, string> Metadata,
            ProviderUnion ProviderSelection,
            string Status)
            : MandateDetailBase(
                Id,
                Currency,
                Beneficiary,
                Reference,
                User,
                CreatedAt,
                Constraints,
                Metadata,
                ProviderSelection,
                Status)
        {
            const string Discriminator = "authorization_required";
        }

        [JsonDiscriminator(Discriminator)]
        public record AuthorizingMandateDetail(
            string Id,
            string Currency,
            BeneficiaryUnion Beneficiary,
            string Reference,
            PaymentUser User,
            DateTime CreatedAt,
            Constraints Constraints,
            Dictionary<string, string> Metadata,
            ProviderUnion ProviderSelection,
            string Status)
            : MandateDetailBase(
                Id,
                Currency,
                Beneficiary,
                Reference,
                User,
                CreatedAt,
                Constraints,
                Metadata,
                ProviderSelection,
                Status)
        {
            const string Discriminator = "authorizing";
        }

        [JsonDiscriminator(Discriminator)]
        public record AuthorizedMandateDetail(
            string Id,
            string Currency,
            BeneficiaryUnion Beneficiary,
            string Reference,
            PaymentUser User,
            DateTime CreatedAt,
            Constraints Constraints,
            Dictionary<string, string> Metadata,
            ProviderUnion ProviderSelection,
            DateTime AuthorizedAt,
            RemitterAccount Remitter,
            string Status)
            : MandateDetailBase(
                Id,
                Currency,
                Beneficiary,
                Reference,
                User,
                CreatedAt,
                Constraints,
                Metadata,
                ProviderSelection,
                Status)
        {
            const string Discriminator = "authorized";
        }

        [JsonDiscriminator(Discriminator)]
        public record FailedMandateDetail(
            string Id,
            string Currency,
            BeneficiaryUnion Beneficiary,
            string Reference,
            PaymentUser User,
            DateTime CreatedAt,
            Constraints Constraints,
            Dictionary<string, string> Metadata,
            ProviderUnion ProviderSelection,
            string FailureStage,
            string FailureReason,
            DateTime AuthorizationFailedAt,
            string Status)
            : MandateDetailBase(
                Id,
                Currency,
                Beneficiary,
                Reference,
                User,
                CreatedAt,
                Constraints,
                Metadata,
                ProviderSelection,
                Status)
        {
            const string Discriminator = "failed";
        }

        [JsonDiscriminator(Discriminator)]
        public record RevokedMandateDetail(
            string Id,
            string Currency,
            BeneficiaryUnion Beneficiary,
            string Reference,
            PaymentUser User,
            DateTime CreatedAt,
            Constraints Constraints,
            Dictionary<string, string> Metadata,
            ProviderUnion ProviderSelection,
            string RevocationSource,
            DateTime AuthorizedAt,
            DateTime RevokedAt,
            RemitterAccount Remitter,
            string Status)
            : MandateDetailBase(
                Id,
                Currency,
                Beneficiary,
                Reference,
                User,
                CreatedAt,
                Constraints,
                Metadata,
                ProviderSelection,
                Status)
        {
            const string Discriminator = "revoked";
        }
    }
}
