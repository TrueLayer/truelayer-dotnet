using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using static TrueLayer.Payments.Model.Provider;
using static TrueLayer.Payments.Model.Beneficiary;
using TrueLayer.Serialization;

namespace TrueLayer.Mandates.Model
{
    using Provider = OneOf<UserSelected, Preselected>;
    using Beneficiary = OneOf<ExternalAccount, MerchantAccount>;

    [JsonDiscriminator(Discriminator)]
    public record VRPCommercialMandate(
        string Type,
        Provider Provider,
        Beneficiary Beneficiary,
        string Reference) : IDiscriminated
    {
        const string Discriminator = "commercial";
    }

    [JsonDiscriminator(Discriminator)]
    public record VRPSweepingMandate(
        string Type,
        Provider Provider,
        Beneficiary Beneficiary,
        string Reference) : IDiscriminated
    {
        const string Discriminator = "sweeping";
    }
}
