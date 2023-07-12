using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Payments.Model.AccountIdentifier;

namespace TrueLayer.Mandates.Model
{
    using AccountIdentifier = OneOf<SortCodeAccountNumber, Iban, Bban, Nrb>;

    public static class Beneficiary
    {
        [JsonDiscriminator(Discriminator)]
        public record MerchantAccount(
            string Type,
            string MerchantAccountId,
            string AccountHolderName) : IDiscriminated
        {
            const string Discriminator = "merchant_account";
        }

        [JsonDiscriminator(Discriminator)]
        public record ExternalAccount(
            string Type,
            string AccountHolderName,
            AccountIdentifier AccountIdentifier) : IDiscriminated
        {
            const string Discriminator = "external_account";
        }
    }
}
