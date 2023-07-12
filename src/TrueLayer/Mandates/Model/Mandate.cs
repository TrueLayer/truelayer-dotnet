using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Mandates.Model.Beneficiary;
using static TrueLayer.Mandates.Model.Provider;

namespace TrueLayer.Mandates.Model
{
    using ProviderUnion = OneOf<Payments.Model.Provider.UserSelected, Preselected>;
    using BeneficiaryUnion = OneOf<ExternalAccount, MerchantAccount>;

    public static class Mandate
    {
        [JsonDiscriminator(Discriminator)]
        public record VRPCommercialMandate(
            string Type,
            ProviderUnion Provider,
            BeneficiaryUnion Beneficiary,
            string Reference) : IDiscriminated
        {
            const string Discriminator = "commercial";
        }

        [JsonDiscriminator(Discriminator)]
        public record VRPSweepingMandate(
            string Type,
            ProviderUnion Provider,
            BeneficiaryUnion Beneficiary,
            string Reference) : IDiscriminated
        {
            const string Discriminator = "sweeping";
        }
    }
}
