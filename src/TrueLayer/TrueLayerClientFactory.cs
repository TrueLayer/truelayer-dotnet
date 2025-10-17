using System;
using Microsoft.Extensions.Options;
using TrueLayer.Auth;
using TrueLayer.Caching;
using TrueLayer.Mandates;
using TrueLayer.MerchantAccounts;
using TrueLayer.Payments;
using TrueLayer.PaymentsProviders;
using TrueLayer.Payouts;

namespace TrueLayer;

internal class TrueLayerClientFactory
{
    private readonly IApiClient _apiClient;
    private readonly IOptionsFactory<TrueLayerOptions> _options;
    private readonly IAuthTokenCache _authTokenCache;

    public TrueLayerClientFactory(IApiClient apiClient, IOptionsFactory<TrueLayerOptions> options, IAuthTokenCache authTokenCache)
    {
        options.NotNull(nameof(options));
        _apiClient = apiClient;
        _options = options;
        _authTokenCache = authTokenCache;
    }

    public ITrueLayerClient Create(string configName)
    {
        var options = _options.Create(configName);
        var auth = new AuthApi(_apiClient, options);

        return new TrueLayerClient(
            auth,
            new Lazy<IPaymentsApi>(() => new PaymentsApi(_apiClient, auth, options)),
            new Lazy<IPaymentsProvidersApi>(() => new PaymentsProvidersApi(_apiClient, auth, options)),
            new Lazy<IPayoutsApi>(() => new PayoutsApi(_apiClient, auth, options)),
            new Lazy<IMerchantAccountsApi>(() => new MerchantAccountsApi(_apiClient, auth, options)),
            new Lazy<IMandatesApi>(() => new MandatesApi(_apiClient, auth, options)));
    }

    public ITrueLayerClient CreateWithCache(string configName)
    {
        var options =_options.Create(configName);
        var decoratedAuthApi = new AuthApiCacheDecorator(new AuthApi(_apiClient, options), _authTokenCache, options);

        return new TrueLayerClient(
            decoratedAuthApi,
            new Lazy<IPaymentsApi>(() => new PaymentsApi(_apiClient, decoratedAuthApi, options)),
            new Lazy<IPaymentsProvidersApi>(() => new PaymentsProvidersApi(_apiClient, decoratedAuthApi, options)),
            new Lazy<IPayoutsApi>(() => new PayoutsApi(_apiClient, decoratedAuthApi, options)),
            new Lazy<IMerchantAccountsApi>(() => new MerchantAccountsApi(_apiClient, decoratedAuthApi, options)),
            new Lazy<IMandatesApi>(() => new MandatesApi(_apiClient, decoratedAuthApi, options)));
    }
}