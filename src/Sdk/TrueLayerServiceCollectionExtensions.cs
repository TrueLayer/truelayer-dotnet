using System;
using Microsoft.Extensions.Configuration;
using TrueLayer;
using TrueLayer.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// This class adds extension methods to IServiceCollection making it easier to add the TrueLayer client
    /// to the NET Core dependency injection framework.
    /// </summary>
    public static class TrueLayerServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the default TrueLayer SDK services to the provided <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="options">The TrueLayer options.</param>
        /// <param name="configureBuilder">Action to override the HttpClientBuilder.</param>
        /// <returns>The service collection with registered TrueLayer SDK services.</returns>
        public static IServiceCollection AddTrueLayerSdk(this IServiceCollection services,
            TrueLayerOptions options, Action<IHttpClientBuilder>? configureBuilder = null)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (options is null) throw new ArgumentNullException(nameof(options));

            IHttpClientBuilder httpClientBuilder = services.AddHttpClient<IApiClient, ApiClient>()
                .AddHttpMessageHandler(() => new UserAgentHandler());
            
            configureBuilder?.Invoke(httpClientBuilder);

            services.AddSingleton<ISerializer>(new JsonSerializer());
            services.AddTransient<ITrueLayerApi, TrueLayerApi>();
            services.AddSingleton(options);

            return services;
        }

        /// <summary>
        /// Registers the default TrueLayer SDK services to the provided <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configuration">The Microsoft configuration used to obtain the TrueLayer SDK configuration.</param>
        /// <param name="configureOptions">Action to customise the TrueLayer options created from configuration.</param>
        /// <param name="configureBuilder">Action to override the HttpClientBuilder.</param>
        /// <returns>The service collection with registered TrueLayer SDK services.</returns>
        public static IServiceCollection AddTrueLayerSdk(this IServiceCollection services,
            IConfiguration configuration, Action<TrueLayerOptions>? configureOptions = null, Action<IHttpClientBuilder>? configureBuilder = null)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            TrueLayerOptions options = configuration.GetTrueLayerOptions();
            configureOptions?.Invoke(options);
            options.Validate();
            return services.AddTrueLayerSdk(options, configureBuilder);
        }
    }
}
