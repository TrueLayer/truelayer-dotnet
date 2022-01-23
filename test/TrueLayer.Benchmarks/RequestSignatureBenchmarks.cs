using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;

namespace TrueLayer.Benchmarks
{
    [MemoryDiagnoser]
    public class RequestSignatureBenchmarks
    {
        private static string PrivateKey = @"-----BEGIN EC PRIVATE KEY-----
MIHcAgEBBEIALJ2sKM+8mVDfTIlk50rqB5lkxaLBt+OECvhXq3nEaB+V0nqljZ9c
5aHRN3qqxMzNLvxFQ+4twifa4ezkMK2/j5WgBwYFK4EEACOhgYkDgYYABADmhZbj
i8bgJRfMTdtzy+5VbS5ScMaKC1LQfhII+PTzGzOr+Ts7Qv8My5cmYU5qarGK3tWF
c3VMlcFZw7Y0iLjxAQFPvHqJ9vn3xWp+d3JREU1vQJ9daXswwbcoer88o1oVFmFf
WS1/11+TH1x/lgKckAws6sAzJLPtCUZLV4IZTb6ENg==
-----END EC PRIVATE KEY-----";

        private static string Json = JsonSerializer.Serialize(CreatePaymentRequest(), SerializerOptions.Default);
        private static SigningKey SigningKey = new SigningKey { KeyId = Guid.NewGuid().ToString(), PrivateKey = PrivateKey };

        [Benchmark]
        public string Create()
            => RequestSignature.Create(
                SigningKey,
                HttpMethod.Post,
                new Uri("http://localhost/payments"),
                Json,
                "idempotency-key"
            );

        private static CreatePaymentRequest CreatePaymentRequest()
            => new CreatePaymentRequest(
                100,
                Currencies.GBP,
                new PaymentMethod.BankTransfer
                {
                    ProviderFilter = new ProviderFilter
                    {
                        ProviderIds = new[] { "mock-payments-gb-redirect" }
                    }
                },
                new Beneficiary.ExternalAccount(
                    "TrueLayer",
                    "truelayer-dotnet",
                    new SchemeIdentifier.SortCodeAccountNumber("567890", "12345678")
                ),
                PaymentUserRequest.Existing(Guid.NewGuid().ToString(), "Jane Doe", "jane.doe@example.com", "+442079460087")
            );
    }
}
