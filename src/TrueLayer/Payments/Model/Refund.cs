using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    public abstract record RefundBase
    {
        public uint? AmountInMinor { get; init; }

        public string Currency { get; init; } = null!;
    }

    [JsonDiscriminator("pending")]
    public record Pending : RefundBase
    {
        public string Status => "pending";
    };

    [JsonDiscriminator("authorized")]
    public record Authorized : RefundBase
    {
        public string Status => "authorized";
    };
}

