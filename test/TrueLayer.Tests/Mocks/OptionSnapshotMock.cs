using Microsoft.Extensions.Options;

namespace TrueLayer.Tests.Mocks;

public class OptionSnapshotMock : IOptionsSnapshot<TrueLayerOptions>
{
    public OptionSnapshotMock(TrueLayerOptions options)
    {
        Value = options;
    }
    public TrueLayerOptions Value { get; }
    public TrueLayerOptions Get(string? name)
    {
        return Value;
    }
}
