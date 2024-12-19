using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TrueLayer;
using TrueLayer.Auth;

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
        /// <param name="configuration">The Microsoft configuration used to obtain the TrueLayer SDK configuration.</param>
        /// <param name="configureOptions">Action to customise the TrueLayer options created from configuration.</param>
        /// <param name="configureBuilder">Action to override the HttpClientBuilder.</param>
        /// <param name="configurationSectionName">Name of configuration section used to build the TrueLayer client</param>
        /// <param name="authCachingStrategy">Caching strategy for auth token</param>
        /// <returns>The service collection with registered TrueLayer SDK services.</returns>
        public static IServiceCollection AddTrueLayer(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<TrueLayerOptions>? configureOptions = null,
            Action<IHttpClientBuilder>? configureBuilder = null,
            string configurationSectionName = "TrueLayer",
            AuthCachingStrategy authCachingStrategy = AuthCachingStrategy.None)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<TrueLayerOptions>(options =>
            {
                configuration.GetSection(configurationSectionName).Bind(options);
                configureOptions?.Invoke(options);
                options.Validate();
            });

            IHttpClientBuilder httpClientBuilder = services.AddHttpClient<IApiClient, ApiClient>();
            configureBuilder?.Invoke(httpClientBuilder);

            services.AddTransient<TrueLayerClientFactory>();

            switch (authCachingStrategy)
            {
                case AuthCachingStrategy.None:
                    services.AddSingleton<IAuthTokenCache, NullMemoryCache>();
                    services.AddTransient<ITrueLayerClient>(s => s.GetRequiredService<TrueLayerClientFactory>().Create());
                    break;
                case AuthCachingStrategy.InMemory:
                    services.AddMemoryCache();
                    services.AddSingleton<IAuthTokenCache, InMemoryAuthTokenCache>();
                    services.AddTransient<ITrueLayerClient>(x => x.GetRequiredService<TrueLayerClientFactory>().CreateWithCache());
                    break;
                case AuthCachingStrategy.Custom:
                    services.AddTransient<ITrueLayerClient>(x => x.GetRequiredService<TrueLayerClientFactory>().CreateWithCache());
                    break;
            }

            return services;
        }

        /// <summary>
        /// Registers the default TrueLayer SDK services to the provided <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configuration">The Microsoft configuration used to obtain the TrueLayer SDK configuration.</param>
        /// <param name="configureOptions">Action to customise the TrueLayer options created from configuration.</param>
        /// <param name="configureBuilder">Action to override the HttpClientBuilder.</param>
        /// <param name="configurationSectionName">Name of configuration section used to build the TrueLayer client</param>
        /// <param name="serviceKey">Key used to register TrueLayer client in DI</param>
        /// <param name="authCachingStrategy">Caching strategy for auth token</param>
        /// <returns>The service collection with registered TrueLayer SDK services.</returns>
        public static IServiceCollection AddKeyedTrueLayer(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<TrueLayerOptions>? configureOptions = null,
            Action<IHttpClientBuilder>? configureBuilder = null,
            string configurationSectionName = "TrueLayer",
            string serviceKey = "TrueLayerClient",
            AuthCachingStrategy authCachingStrategy = AuthCachingStrategy.None)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<TrueLayerOptions>(serviceKey, options =>
            {
                configuration.GetSection(configurationSectionName).Bind(options);
                configureOptions?.Invoke(options);
                options.Validate();
            });

            IHttpClientBuilder httpClientBuilder = services.AddHttpClient<IApiClient, ApiClient>();
            configureBuilder?.Invoke(httpClientBuilder);

            services.AddKeyedTransient<TrueLayerClientFactory>(serviceKey);

            switch (authCachingStrategy)
            {
                case AuthCachingStrategy.None:
                    services.AddKeyedSingleton<IAuthTokenCache, NullMemoryCache>(serviceKey);
                    services.AddKeyedTransient<ITrueLayerClient>(serviceKey,
                        (x, _) => x.GetRequiredKeyedService<TrueLayerClientFactory>(serviceKey)
                            .CreateKeyed(serviceKey));
                    break;
                case AuthCachingStrategy.InMemory:
                    services.AddMemoryCache();
                    services.AddKeyedSingleton<IAuthTokenCache, InMemoryAuthTokenCache>(serviceKey);
                    services.AddKeyedTransient<ITrueLayerClient>(serviceKey,
                        (x, _) => x.GetRequiredKeyedService<TrueLayerClientFactory>(serviceKey)
                            .CreateWithCacheKeyed(serviceKey));
                    break;
                case AuthCachingStrategy.Custom:
                    services.AddKeyedTransient<ITrueLayerClient>(serviceKey,
                        (x, _) => x.GetRequiredKeyedService<TrueLayerClientFactory>(serviceKey)
                            .CreateWithCacheKeyed(serviceKey));
                    break;
            }

            return services;
        }
    }
}
