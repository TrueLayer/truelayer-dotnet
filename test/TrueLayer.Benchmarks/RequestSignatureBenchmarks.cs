using System;
using System.Net.Http;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;
using TrueLayer.Signing;

namespace TrueLayer.Benchmarks
{
    [MemoryDiagnoser]
    public class RequestSignatureBenchmarks
    {
        private const string PrivateKey = @"-----BEGIN EC PRIVATE KEY-----
MIHcAgEBBEIALJ2sKM+8mVDfTIlk50rqB5lkxaLBt+OECvhXq3nEaB+V0nqljZ9c
5aHRN3qqxMzNLvxFQ+4twifa4ezkMK2/j5WgBwYFK4EEACOhgYkDgYYABADmhZbj
i8bgJRfMTdtzy+5VbS5ScMaKC1LQfhII+PTzGzOr+Ts7Qv8My5cmYU5qarGK3tWF
c3VMlcFZw7Y0iLjxAQFPvHqJ9vn3xWp+d3JREU1vQJ9daXswwbcoer88o1oVFmFf
WS1/11+TH1x/lgKckAws6sAzJLPtCUZLV4IZTb6ENg==
-----END EC PRIVATE KEY-----";

        private static readonly string Json = JsonSerializer.Serialize(CreatePaymentRequest(), SerializerOptions.Default);
        private static readonly SigningKey SigningKey = new() { KeyId = Guid.NewGuid().ToString(), PrivateKey = PrivateKey };

        [Benchmark]
        public string Create()
            => Signer.SignWith(SigningKey.KeyId, SigningKey.Value)
                .Method(HttpMethod.Post.Method)
                .Path("payments")
                .Body(Json)
                .Header(CustomHeaders.IdempotencyKey, "idempotency-key")
                .Sign();

        private static CreatePaymentRequest CreatePaymentRequest()
            => new(
                100,
                Currencies.GBP,
                new PaymentMethod.BankTransfer(
                    new Provider.UserSelected
                    {
                        Filter = new ProviderFilter { ProviderIds = new[] { "mock-payments-gb-redirect" } }
                    },
                    new Beneficiary.ExternalAccount(
                        "TrueLayer",
                        "truelayer-dotnet",
                        new AccountIdentifier.SortCodeAccountNumber("567890", "12345678")
                    )
                ),
                new PaymentUserRequest(id: Guid.NewGuid().ToString())
            );
    }
}
