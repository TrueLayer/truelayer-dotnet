# Error Handling

All TrueLayer API calls return an `ApiResponse` or `ApiResponse<T>` that includes status information and error details.

## Checking for Success

```csharp
var response = await _client.Payments.CreatePayment(request, idempotencyKey);

if (response.IsSuccessful)
{
    // Handle success
    var payment = response.Data;
}
else
{
    // Handle failure
    var statusCode = response.StatusCode;
    var problemDetails = response.Problem;
}
```

## Problem Details

When a request fails, the `Problem` property contains detailed error information:

```csharp
if (!response.IsSuccessful && response.Problem != null)
{
    Console.WriteLine($"Error: {response.Problem.Title}");
    Console.WriteLine($"Detail: {response.Problem.Detail}");
    Console.WriteLine($"Type: {response.Problem.Type}");
    Console.WriteLine($"TraceId: {response.TraceId}");

    if (response.Problem.Errors != null)
    {
        foreach (var (field, messages) in response.Problem.Errors)
        {
            Console.WriteLine($"{field}: {string.Join(", ", messages)}");
        }
    }
}
```

## Common HTTP Status Codes

| Code | Meaning | Action |
|------|---------|--------|
| 200-299 | Success | Process response data |
| 400 | Bad Request | Check request validation errors |
| 401 | Unauthorized | Verify credentials |
| 403 | Forbidden | Check API permissions |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Handle idempotency key collision |
| 429 | Rate Limited | Implement retry with backoff |
| 500-599 | Server Error | Retry with exponential backoff |

## Using TraceId for Support

Always log the `TraceId` for failed requests - TrueLayer support needs this to investigate issues:

```csharp
_logger.LogError(
    "Payment creation failed. TraceId: {TraceId}, Status: {StatusCode}, Error: {Error}",
    response.TraceId,
    response.StatusCode,
    response.Problem?.Detail
);
```

## Working with OneOf Response Types

Many API methods return discriminated unions using `OneOf`:

```csharp
var response = await _client.Payments.GetPayment(paymentId);

if (response.IsSuccessful)
{
    var result = response.Data.Match(
        authRequired => $"Auth required: {authRequired.Status}",
        authorizing => $"Authorizing: {authorizing.Status}",
        authorized => $"Authorized: {authorized.Status}",
        executed => $"Executed: {executed.Status}",
        settled => $"Settled: {settled.Status}",
        failed => $"Failed: {failed.FailureReason}",
        attemptFailed => $"Attempt failed: {attemptFailed.FailureReason}"
    );
}
```

## Exception Handling

The SDK doesn't throw exceptions for API errors - always check `IsSuccessful`. However, network errors and configuration issues may throw:

```csharp
try
{
    var response = await _client.Payments.CreatePayment(request, idempotencyKey);

    if (!response.IsSuccessful)
    {
        // Handle API error
        return Results.Problem(
            detail: response.Problem?.Detail,
            statusCode: (int)response.StatusCode
        );
    }

    return Results.Ok(response.Data);
}
catch (HttpRequestException ex)
{
    // Network error
    _logger.LogError(ex, "Network error calling TrueLayer API");
    return Results.Problem("Service temporarily unavailable");
}
catch (TaskCanceledException ex)
{
    // Timeout
    _logger.LogError(ex, "Request to TrueLayer API timed out");
    return Results.Problem("Request timed out");
}
```

## Validation Errors

Field-level validation errors are in `Problem.Errors`:

```csharp
if (response.StatusCode == HttpStatusCode.BadRequest && response.Problem?.Errors != null)
{
    foreach (var (field, messages) in response.Problem.Errors)
    {
        modelState.AddModelError(field, string.Join("; ", messages));
    }
    return View(model);
}
```

## Retry Strategies

For transient failures, implement retry logic:

```csharp
public async Task<ApiResponse<T>> WithRetry<T>(
    Func<Task<ApiResponse<T>>> operation,
    int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            var response = await operation();

            // Retry on server errors or rate limiting
            if (response.StatusCode == HttpStatusCode.TooManyRequests ||
                (int)response.StatusCode >= 500)
            {
                if (i < maxRetries - 1)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, i));
                    await Task.Delay(delay);
                    continue;
                }
            }

            return response;
        }
        catch (HttpRequestException)
        {
            if (i == maxRetries - 1) throw;
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
        }
    }

    throw new InvalidOperationException("Retry logic error");
}
```

## Best Practices

1. **Always check `IsSuccessful`** before accessing `Data`
2. **Log `TraceId`** for all failed requests
3. **Handle all union cases** in `Match()` calls
4. **Don't swallow errors** - log and handle appropriately
5. **Use structured logging** with status codes and trace IDs
6. **Implement retries** for transient failures (429, 5xx)
7. **Validate input** before making API calls

## See Also

- [Payments Guide](payments.md)
- [API Reference](xref:TrueLayer.ApiResponse)
