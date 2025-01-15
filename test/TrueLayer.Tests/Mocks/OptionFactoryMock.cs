using Microsoft.Extensions.Options;

namespace TrueLayer.Tests.Mocks;

public class OptionFactoryMock : IOptionsFactory<TrueLayerOptions>
{
    public OptionFactoryMock(TrueLayerOptions options)
    {
        Value = options;
    }
    public TrueLayerOptions Value { get; }
    public TrueLayerOptions Create(string name)
    {
        return Value;
    }
}
