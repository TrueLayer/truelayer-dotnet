using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;
using static TrueLayer.Payments.Model.Provider;
using static TrueLayer.Payments.Model.Beneficiary;
using System.Reflection.Emit;

namespace TrueLayer.Mandates.Model
{
    using Provider = OneOf<UserSelected, Preselected>;
    using Beneficiary = OneOf<ExternalAccount, MerchantAccount>;

    public abstract record MandateDetail(
        string Id,
        string Currency,
        Beneficiary Beneficiary,
        string Reference,
        PaymentUser User,
        DateTime CreatedAt,
        Constraints Constraints,
        Dictionary<string, string> Metadata,
        Provider ProviderSelection,
        string Status);

    [JsonDiscriminator(Discriminator)]
    public record AuthorizationRequiredMandateDetail(
        string Id,
        string Currency,
        Beneficiary Beneficiary,
        string Reference,
        PaymentUser User,
        DateTime CreatedAt,
        Constraints Constraints,
        Dictionary<string, string> Metadata,
        Provider ProviderSelection,
        string Status)
        : MandateDetail(
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
        Beneficiary Beneficiary,
        string Reference,
        PaymentUser User,
        DateTime CreatedAt,
        Constraints Constraints,
        Dictionary<string, string> Metadata,
        Provider ProviderSelection,
        string Status)
        : MandateDetail(
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
        Beneficiary Beneficiary,
        string Reference,
        PaymentUser User,
        DateTime CreatedAt,
        Constraints Constraints,
        Dictionary<string, string> Metadata,
        Provider ProviderSelection,
        DateTime AuthorizedAt,
        RemitterAccount Remitter,
        string Status)
        : MandateDetail(
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
        Beneficiary Beneficiary,
        string Reference,
        PaymentUser User,
        DateTime CreatedAt,
        Constraints Constraints,
        Dictionary<string, string> Metadata,
        Provider ProviderSelection,
        string FailureStage,
        string FailureReason,
        DateTime AuthorizationFailedAt,
        string Status)
        : MandateDetail(
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
        Beneficiary Beneficiary,
        string Reference,
        PaymentUser User,
        DateTime CreatedAt,
        Constraints Constraints,
        Dictionary<string, string> Metadata,
        Provider ProviderSelection,
        string RevocationSource,
        DateTime AuthorizedAt,
        DateTime RevokedAt,
        RemitterAccount Remitter,
        string Status)
        : MandateDetail(
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
