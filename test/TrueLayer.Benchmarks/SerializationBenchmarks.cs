using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;

namespace TrueLayer.Benchmarks
{
    [MemoryDiagnoser]
    public class SerializationBenchmarks
    {
        static byte[] Json = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(CreatePaymentResponse(), SerializerOptions.Default));

        [Benchmark(Baseline = true)]
        public async Task<CreatePaymentResponse?> FromString()
        {
            using var stream = new MemoryStream(Json);
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            return JsonSerializer.Deserialize<CreatePaymentResponse>(json, SerializerOptions.Default);
        }

        [Benchmark]
        public async Task<CreatePaymentResponse?> FromStream()
        {
            using var stream = new MemoryStream(Json);
            return await JsonSerializer.DeserializeAsync<CreatePaymentResponse>(stream, SerializerOptions.Default);
        }

        private static CreatePaymentResponse CreatePaymentResponse()
            => new(
                "b0110edb-7964-4e22-b971-060d6445bb43",
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjbGllbnRfaWQiOiJtdmNleGFtcGxlLTVlM2EzMCIsImp0aSI6ImIwMTEwZWRiLTc5NjQtNGUyMi1iOTcxLTA2MGQ2NDQ1YmI0MyIsIm5iZiI6MTYzNDkyNDQ1NiwiZXhwIjoxNjM0OTI1MzU2LCJpc3MiOiJodHRwczovL2FwaS50N3IuZGV2IiwiYXVkIjoiaHR0cHM6Ly9hcGkudDdyLmRldiJ9.Zz7Eg9Aas6Q9uOlTji9xnre1Vzdemtrs40W6hKdgw6M",
                new PaymentUserResponse(Guid.NewGuid().ToString())
            );
    }
}
