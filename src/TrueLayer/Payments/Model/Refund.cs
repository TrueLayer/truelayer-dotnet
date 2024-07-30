using System;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    //TODO complete
    public abstract record RefundBase
    {
        public uint AmountInMinor { get; init; }

        public string Currency { get; init; } = null!;
    }

    [JsonDiscriminator("pending")]
    public sealed record RefundPending : RefundBase
    {
        public string Status => "pending";
    };

    [JsonDiscriminator("authorized")]
    public sealed record RefundAuthorized : RefundBase
    {
        public string Status => "authorized";
    };

    [JsonDiscriminator("executed")]
    public sealed record RefundExecuted : RefundBase
    {
        public string Status => "executed";
        public DateTime ExecutedAt { get; init; }
    };
}

