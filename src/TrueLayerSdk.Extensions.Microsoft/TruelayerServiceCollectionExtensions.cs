using System;
using Microsoft.Extensions.DependencyInjection;
using TrueLayerSdk;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// This class adds extension methods to IServiceCollection making it easier to add the Truelayer client
    /// to the NET Core dependency injection framework.
    /// </summary>
    public static class TruelayerServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the default Truelayer SDK services to the provided <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configuration">The Truelayer configuration.</param>
        /// <returns>The service collection with registered Truelayer SDK services.</returns>
        public static IServiceCollection AddTruelayerSdk(this IServiceCollection services,
            TruelayerConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddSingleton<IHttpClientFactory>(new DefaultHttpClientFactory());
            services.AddSingleton<ISerializer>(new JsonSerializer());
            services.AddSingleton(configuration);
            services.AddSingleton<IApiClient, ApiClient>();
            services.AddSingleton<ITruelayerApi, TruelayerApi>();

            return services;
        }
        
        /// <summary>
        /// Registers the default Truelayer SDK services to the provided <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configuration">The Microsoft configuration used to obtain the Truelayer SDK configuration.</param>
        /// <returns>The service collection with registered Truelayer SDK services.</returns>
        public static IServiceCollection AddTruelayerSdk(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var truelayerOptions = configuration.GetTruelayerOptions();
            return services.AddTruelayerSdk(truelayerOptions.CreateConfiguration());
        }
    }
}
