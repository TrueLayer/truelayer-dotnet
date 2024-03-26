using Microsoft.Extensions.DependencyInjection;
using System;

namespace TrueLayer
{
    public interface ITrueLayerClientResolver
    {
        ITrueLayerClient GetClientByName(string name);
    }

    public class TrueLayerClientResolver : ITrueLayerClientResolver
    {
        private readonly IServiceProvider _serviceProvider;
        public TrueLayerClientResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
#pragma warning disable CS8603 // Possible null reference return.
        public ITrueLayerClient GetClientByName(string name) => _serviceProvider.GetKeyedService<ITrueLayerClient>(name);
#pragma warning restore CS8603 // Possible null reference return.

    }
}
