using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using TrueLayer;

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
        /// <returns>The service collection with registered TrueLayer SDK services.</returns>
        public static IServiceCollection AddTrueLayer(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<TrueLayerOptions>? configureOptions = null,
            Action<IHttpClientBuilder>? configureBuilder = null,
            string configurationSectionName = "TrueLayer")
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

            services.AddSingleton<IAuthTokenCache, NullMemoryCache>();
            services.AddTransient<TrueLayerClientFactory>();
            services.AddTransient<ITrueLayerClient>(s => s.GetRequiredService<TrueLayerClientFactory>().Create());

            return services;
        }

        /// <summary>
        /// Registers the default caching mechanism for the auth token<paramref name="services"/>.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        public static IServiceCollection AddAuthTokenInMemoryCaching(
            this IServiceCollection services)
        {
            var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ImplementationType == typeof(NullMemoryCache));
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);
            }

            services.AddMemoryCache();
            services.AddSingleton<IAuthTokenCache, InMemoryAuthTokenCache>();
            services.AddTransient<ITrueLayerClient>(s => s.GetRequiredService<TrueLayerClientFactory>().CreateWithCache());

            return services;
        }

        /// <summary>
        /// Registers a custom caching mechanism for the auth token<paramref name="services"/>.
        /// You need to register your own implementation of IAuthTokenCache before invoking this method
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        public static IServiceCollection AddAuthTokenCaching(
            this IServiceCollection services)
        {
            var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ImplementationType == typeof(NullMemoryCache));
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);
            }

            services.AddTransient<ITrueLayerClient>(s => s.GetRequiredService<TrueLayerClientFactory>().CreateWithCache());

            return services;
        }
    }
}
