using System;
using System.Security.Cryptography;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace TrueLayer.Sdk.Tests
{
    public class RequestSigningTests
    {
        // docker run --rm -it -v $(pwd):/export --entrypoint /bin/ash alpine/openssl  
        // openssl ecparam -genkey -name secp521r1 -noout -out /export/ec512-private-key.pem
        // openssl ec -in /export/ec512-private-key.pem -pubout -out /export/ec512-public-key.pem

        [Fact]
        public void Can_generate_jws()
        {
            var privateKey = @"-----BEGIN EC PRIVATE KEY-----
MIHcAgEBBEIALJ2sKM+8mVDfTIlk50rqB5lkxaLBt+OECvhXq3nEaB+V0nqljZ9c
5aHRN3qqxMzNLvxFQ+4twifa4ezkMK2/j5WgBwYFK4EEACOhgYkDgYYABADmhZbj
i8bgJRfMTdtzy+5VbS5ScMaKC1LQfhII+PTzGzOr+Ts7Qv8My5cmYU5qarGK3tWF
c3VMlcFZw7Y0iLjxAQFPvHqJ9vn3xWp+d3JREU1vQJ9daXswwbcoer88o1oVFmFf
WS1/11+TH1x/lgKckAws6sAzJLPtCUZLV4IZTb6ENg==
-----END EC PRIVATE KEY-----";

            string json = JsonSerializer.Serialize(new
            {
                foo = "bar"
            });

            string signature = RequestSigning.SignJson(json, new() { Certificate = privateKey, KeyId = Guid.NewGuid().ToString() });
            signature.ShouldNotBeNullOrWhiteSpace();
        }
    }
}


