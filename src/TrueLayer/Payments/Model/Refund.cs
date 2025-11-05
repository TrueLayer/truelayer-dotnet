using System;
using System.Collections.Generic;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model;

/// <summary>
/// Base record for all refund types, containing common refund properties.
/// </summary>
public abstract record RefundBase
{
    /// <summary>
    /// Gets or initializes the unique identifier for the refund.
    /// </summary>
    public string Id { get; init; } = null!;

    /// <summary>
    /// Gets or initializes the reference for the refund.
    /// </summary>
    public string Reference { get; init; } = null!;

    /// <summary>
    /// Gets or initializes the refund amount in minor currency units (e.g., cents for USD).
    /// </summary>
    public uint AmountInMinor { get; init; }

    /// <summary>
    /// Gets or initializes the currency code for the refund.
    /// </summary>
    public string Currency { get; init; } = null!;

    /// <summary>
    /// Gets or initializes the metadata associated with the refund.
    /// </summary>
    public Dictionary<string,string> Metadata { get; init; } = null!;

    /// <summary>
    /// Gets or initializes the status of the refund.
    /// </summary>
    public string Status { get; init; } = null!;

    /// <summary>
    /// Gets or initializes the timestamp when the refund was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Represents a refund that is pending processing.
/// </summary>
[JsonDiscriminator("pending")]
public sealed record RefundPending : RefundBase
{
    /// <summary>
    /// Gets the status of the refund as "pending".
    /// </summary>
    public new string Status => "pending";
}

/// <summary>
/// Represents a refund that has been authorized.
/// </summary>
[JsonDiscriminator("authorized")]
public sealed record RefundAuthorized : RefundBase
{
    /// <summary>
    /// Gets the status of the refund as "authorized".
    /// </summary>
    public new string Status => "authorized";
}

/// <summary>
/// Represents a refund that has been executed successfully.
/// </summary>
[JsonDiscriminator("executed")]
public sealed record RefundExecuted : RefundBase
{
    /// <summary>
    /// Gets the status of the refund as "executed".
    /// </summary>
    public new string Status => "executed";

    /// <summary>
    /// Gets or initializes the timestamp when the refund was executed.
    /// </summary>
    public DateTime ExecutedAt { get; init; }
}

/// <summary>
/// Represents a refund that has failed.
/// </summary>
[JsonDiscriminator("failed")]
public sealed record RefundFailed : RefundBase
{
    /// <summary>
    /// Gets the status of the refund as "failed".
    /// </summary>
    public new string Status => "failed";

    /// <summary>
    /// Gets or initializes the timestamp when the refund failed.
    /// </summary>
    public DateTime FailedAt { get; init; }

    /// <summary>
    /// Gets or initializes the reason why the refund failed.
    /// </summary>
    public string FailureReason { get; init; } = null!;
};