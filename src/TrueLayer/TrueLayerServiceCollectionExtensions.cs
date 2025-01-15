using System;
using Microsoft.Extensions.Configuration;
using TrueLayer;
using TrueLayer.Caching;

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
        /// Support for single client only.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configuration">The Microsoft configuration used to obtain the TrueLayer SDK configuration.</param>
        /// <param name="configureOptions">Action to customise the TrueLayer options created from configuration.</param>
        /// <param name="configureBuilder">Action to override the HttpClientBuilder.</param>
        /// <param name="configurationSectionName">Name of configuration section used to build the TrueLayer client</param>
        /// <param name="authTokenCachingStrategy">Caching strategy for auth token</param>
        /// <returns>The service collection with registered TrueLayer SDK services.</returns>
        public static IServiceCollection AddTrueLayer(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<TrueLayerOptions>? configureOptions = null,
            Action<IHttpClientBuilder>? configureBuilder = null,
            string configurationSectionName = "TrueLayer",
            AuthTokenCachingStrategies authTokenCachingStrategy = AuthTokenCachingStrategies.None)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<TrueLayerOptions>(configurationSectionName, options =>
            {
                configuration.GetSection(configurationSectionName).Bind(options);
                configureOptions?.Invoke(options);
                options.Validate();
            });

            IHttpClientBuilder httpClientBuilder = services.AddHttpClient<IApiClient, ApiClient>();
            configureBuilder?.Invoke(httpClientBuilder);

            services.AddTransient<TrueLayerClientFactory>();

            switch (authTokenCachingStrategy)
            {
                case AuthTokenCachingStrategies.None:
                    services.AddSingleton<IAuthTokenCache, NullMemoryCache>();
                    services.AddTransient<ITrueLayerClient>(s => s.GetRequiredService<TrueLayerClientFactory>().Create(configurationSectionName));
                    break;
                case AuthTokenCachingStrategies.InMemory:
                    services.AddMemoryCache();
                    services.AddSingleton<IAuthTokenCache, InMemoryAuthTokenCache>();
                    services.AddTransient<ITrueLayerClient>(x => x.GetRequiredService<TrueLayerClientFactory>().CreateWithCache(configurationSectionName));
                    break;
                case AuthTokenCachingStrategies.Custom:
                    services.AddTransient<ITrueLayerClient>(x => x.GetRequiredService<TrueLayerClientFactory>().CreateWithCache(configurationSectionName));
                    break;
            }

            return services;
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Registers the keyed TrueLayer SDK services to the provided <paramref name="services"/>.
        /// Required for multi client support.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="serviceKey">Key used to register TrueLayer client in DI.
        /// Also used as <paramref name="configurationSectionName"/> if the parameter is not specified </param>
        /// <param name="configuration">The Microsoft configuration used to obtain the TrueLayer SDK configuration.</param>
        /// <param name="configureOptions">Action to customise the TrueLayer options created from configuration.</param>
        /// <param name="configureBuilder">Action to override the HttpClientBuilder.</param>
        /// <param name="configurationSectionName">Name of configuration section used to build the TrueLayer client</param>
        /// <param name="authTokenCachingStrategy">Caching strategy for auth token</param>
        /// <returns>The service collection with registered TrueLayer SDK services.</returns>
        public static IServiceCollection AddKeyedTrueLayer(
            this IServiceCollection services,
            string serviceKey,
            IConfiguration configuration,
            Action<TrueLayerOptions>? configureOptions = null,
            Action<IHttpClientBuilder>? configureBuilder = null,
            string? configurationSectionName = null,
            AuthTokenCachingStrategies authTokenCachingStrategy = AuthTokenCachingStrategies.None)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            var configName = configurationSectionName ?? serviceKey;
            services.Configure<TrueLayerOptions>(configName, options =>
            {
                configuration.GetSection(configName).Bind(options);
                configureOptions?.Invoke(options);
                options.Validate();
            });

            IHttpClientBuilder httpClientBuilder = services.AddHttpClient<IApiClient, ApiClient>();
            configureBuilder?.Invoke(httpClientBuilder);

            services.AddKeyedTransient<TrueLayerClientFactory>(serviceKey);

            switch (authTokenCachingStrategy)
            {
                case AuthTokenCachingStrategies.None:
                    services.AddSingleton<IAuthTokenCache, NullMemoryCache>();
                    services.AddKeyedTransient<ITrueLayerClient>(serviceKey,
                        (x, _) => x.GetRequiredKeyedService<TrueLayerClientFactory>(serviceKey)
                            .Create(configName));
                    break;
                case AuthTokenCachingStrategies.InMemory:
                    services.AddMemoryCache();
                    services.AddSingleton<IAuthTokenCache, InMemoryAuthTokenCache>();
                    services.AddKeyedTransient<ITrueLayerClient>(serviceKey,
                        (x, _) => x.GetRequiredKeyedService<TrueLayerClientFactory>(serviceKey)
                            .CreateWithCache(configName));
                    break;
                case AuthTokenCachingStrategies.Custom:
                    services.AddKeyedTransient<ITrueLayerClient>(serviceKey,
                        (x, _) => x.GetRequiredKeyedService<TrueLayerClientFactory>(serviceKey)
                            .CreateWithCache(configName));
                    break;
            }

            return services;
        }
#endif
    }
}
