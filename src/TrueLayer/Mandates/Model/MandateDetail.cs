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
    using Status = OneOf<AuthorizationRequired, Authorizing, Authorized, Failed, Revoked>;
    using FailureStage = OneOf<AuthorizationRequired, Authorizing, Authorized>;

    public static class StatusLabels
    {
        public const string AuthorizationRequired = "authorization_required";
        public const string Authorizing = "authorizing";
        public const string Authorized  = "authorized";
        public const string Failed = "failed";
        public const string Revoked = "revoked";
    }

    [JsonDiscriminator(StatusLabels.AuthorizationRequired)]
    public record AuthorizationRequired { public string Label => StatusLabels.AuthorizationRequired; }

    [JsonDiscriminator(StatusLabels.Authorizing)]
    public record Authorizing { public string Label => StatusLabels.Authorizing; }

    [JsonDiscriminator(StatusLabels.Authorized)]
    public record Authorized { public string Label => StatusLabels.Authorized; }

    [JsonDiscriminator(StatusLabels.Failed)]
    public record Failed { public string Label => StatusLabels.Failed; }

    [JsonDiscriminator(StatusLabels.Revoked)]
    public record Revoked { public string Label => StatusLabels.Revoked; }

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
        Status status
    ) : IDiscriminated
    {
        public string Type => status switch
        {
            var s when s.IsT0 => s.AsT0.Label,
            var s when s.IsT1 => s.AsT1.Label,
            var s when s.IsT2 => s.AsT2.Label,
            var s when s.IsT3 => s.AsT3.Label,
            var s when s.IsT4 => s.AsT4.Label,
            _ => throw new NotImplementedException()
        };
    }

    [JsonDiscriminator(StatusLabels.AuthorizationRequired)]
    public record AuthorizationRequiredMandateDetail(
        string Id,
        string Currency,
        Beneficiary Beneficiary,
        string Reference,
        PaymentUser User,
        DateTime CreatedAt,
        Constraints Constraints,
        Dictionary<string, string> Metadata,
        Provider ProviderSelection)
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
            Status.FromT0(new AuthorizationRequired())
        );

    [JsonDiscriminator(StatusLabels.Authorizing)]
    public record AuthorizingMandateDetail(
        string Id,
        string Currency,
        Beneficiary Beneficiary,
        string Reference,
        PaymentUser User,
        DateTime CreatedAt,
        Constraints Constraints,
        Dictionary<string, string> Metadata,
        Provider ProviderSelection)
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
            Status.FromT1(new Authorizing())
        );

    [JsonDiscriminator(StatusLabels.Authorized)]
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
        RemitterAccount Remitter)
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
            Status.FromT2(new Authorized())
        );

    [JsonDiscriminator(StatusLabels.Failed)]
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
        FailureStage FailureStage,
        string FailureReason,
        DateTime AuthorizationFailedAt)
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
            Status.FromT3(new Failed())
        );

    [JsonDiscriminator(StatusLabels.Revoked)]
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
        RemitterAccount Remitter)
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
            Status.FromT4(new Revoked())
        );
}
