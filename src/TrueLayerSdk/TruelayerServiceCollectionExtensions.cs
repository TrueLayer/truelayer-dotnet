﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using TrueLayerSdk;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// This class adds extension methods to IServiceCollection making it easier to add the Truelayer client
    /// to the NET Core dependency injection framework.
    /// </summary>
    public static class TruelayerServiceCollectionExtensions
    {
        private static Action<HttpClient> NullOpHttpClient = _ => { };

        /// <summary>
        /// Registers the default Truelayer SDK services to the provided <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configuration">The Truelayer configuration.</param>
        /// <returns>The service collection with registered Truelayer SDK services.</returns>
        public static IServiceCollection AddTruelayerSdk(this IServiceCollection services,
            TruelayerConfiguration configuration, Action<HttpClient> configureHttpClient = null)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            services.AddHttpClient<ApiClient>(configureHttpClient ?? NullOpHttpClient)
                .AddHttpMessageHandler<UserAgentHandler>();

            services.AddSingleton<ISerializer>(new JsonSerializer());
            services.AddTransient<IApiClient, ApiClient>();
            services.AddTransient<ITruelayerApi, TruelayerApi>();
            services.AddSingleton(configuration);

            return services;
        }

        /// <summary>
        /// Registers the default Truelayer SDK services to the provided <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configuration">The Microsoft configuration used to obtain the Truelayer SDK configuration.</param>
        /// <returns>The service collection with registered Truelayer SDK services.</returns>
        public static IServiceCollection AddTruelayerSdk(this IServiceCollection services,
            IConfiguration configuration, Action<HttpClient> configureHttpClient = null)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            TruelayerOptions truelayerOptions = configuration.GetTruelayerOptions();
            return services.AddTruelayerSdk(truelayerOptions.CreateConfiguration(), configureHttpClient);
        }
    }
}
