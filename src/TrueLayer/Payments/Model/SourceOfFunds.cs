using OneOf;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    using SchemeIdentifiersUnion = OneOf<
        SchemeIdentifier.SortCodeAccountNumber,
        SchemeIdentifier.Bban,
        SchemeIdentifier.Iban,
        SchemeIdentifier.Nrb
    >;

    public static class SourceOfFunds
    {
        [JsonDiscriminator(ExternalAccount.Discriminator)]
        public sealed record ExternalAccount(SchemeIdentifiersUnion[] SchemeIdentifiers)
        {
            public const string Discriminator = "external_account";
            public string Type => Discriminator;
        }
    }
}
