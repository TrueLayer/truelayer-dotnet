using System;
using System.Collections.Generic;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    public abstract record RefundBase
    {
        public string Id { get; init; } = null!;

        public string Reference { get; init; } = null!;

        public uint AmountInMinor { get; init; }

        public string Currency { get; init; } = null!;

        public Dictionary<string,string> Metadata { get; init; } = null!;

        public string Status { get; init; } = null!;

        public DateTime CreatedAt { get; init; }
    }

    [JsonDiscriminator("pending")]
    public sealed record RefundPending : RefundBase
    {
        public string Status => "pending";
    }

    [JsonDiscriminator("authorized")]
    public sealed record RefundAuthorized : RefundBase
    {
        public string Status => "authorized";
    }

    [JsonDiscriminator("executed")]
    public sealed record RefundExecuted : RefundBase
    {
        public string Status => "executed";
        public DateTime ExecutedAt { get; init; }
    }

    [JsonDiscriminator("failed")]
    public sealed record RefundFailed : RefundBase
    {
        public string Status => "failed";
        public DateTime FailedAt { get; init; }
        public string FailureReason { get; init; } = null!;
    };

}

