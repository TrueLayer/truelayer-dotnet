using System;
using Microsoft.Extensions.Configuration;
using TrueLayer;
using TrueLayer.Serialization;

namespace Microsoft.Extensions.DependencyInjection
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
        /// <param name="configureBuilder">Action to override the HttpClientBuilder.</param>
        /// <returns>The service collection with registered Truelayer SDK services.</returns>
        public static IServiceCollection AddTruelayerSdk(this IServiceCollection services,
            TruelayerConfiguration configuration, Action<IHttpClientBuilder> configureBuilder = null)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            IHttpClientBuilder httpClientBuilder = services.AddHttpClient<IApiClient, ApiClient>()
                .AddHttpMessageHandler(() => new UserAgentHandler());
            
            configureBuilder?.Invoke(httpClientBuilder);

            services.AddSingleton<ISerializer>(new JsonSerializer());
            services.AddTransient<ITrueLayerApi, TrueLayerApi>();
            services.AddSingleton(configuration);

            return services;
        }

        /// <summary>
        /// Registers the default Truelayer SDK services to the provided <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configuration">The Microsoft configuration used to obtain the Truelayer SDK configuration.</param>
        /// <param name="configureBuilder">Action to override the HttpClientBuilder.</param>
        /// <returns>The service collection with registered Truelayer SDK services.</returns>
        public static IServiceCollection AddTruelayerSdk(this IServiceCollection services,
            IConfiguration configuration, Action<IHttpClientBuilder> configureBuilder = null)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            TruelayerOptions truelayerOptions = configuration.GetTruelayerOptions();
            return services.AddTruelayerSdk(truelayerOptions.CreateConfiguration(), configureBuilder);
        }
    }
}
