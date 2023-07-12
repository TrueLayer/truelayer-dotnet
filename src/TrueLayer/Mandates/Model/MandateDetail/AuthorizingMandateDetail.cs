using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Payments.Model;
using static TrueLayer.Payments.Model.Provider;
using static TrueLayer.Payments.Model.Beneficiary;
using TrueLayer.Serialization;

namespace TrueLayer.Mandates.Model.MandateDetail
{
    using Provider = OneOf<UserSelected, Preselected>;
    using Beneficiary = OneOf<ExternalAccount, MerchantAccount>;

    [JsonDiscriminator(Status.Authorizing)]
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
        string Type = Status.Authorizing)
        : MandateDetail(
            Id,
            Currency,
            Beneficiary,
            Reference,
            User,
            CreatedAt,
            Constraints,
            Metadata,
            ProviderSelection
        ), IDiscriminated;
}
