using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Auth.Model;

namespace TrueLayer.Auth
{
    /// <summary>
    /// Default implementation of <see cref="IAuthClient"/>.
    /// </summary>
    internal class AuthClient : IAuthClient
    {
        internal const string ProdUrl = "https://auth.truelayer.com/";
        internal const string SandboxUrl = "https://auth.truelayer-sandbox.com/";

        private readonly IApiClient _apiClient;
        private readonly TrueLayerOptions _options;
        internal readonly Uri BaseUri;
        
        public AuthClient(IApiClient apiClient, TrueLayerOptions options)
        {
            _apiClient = apiClient;
            _options = options;

            BaseUri = options.Auth?.Uri ?? 
                       new Uri((options.UseSandbox ?? true) ? SandboxUrl : ProdUrl);
        }

        public async Task<GetAuthUriResponse> GetAuthUri(GetAuthUriRequest request)
        {           
            var response = new GetAuthUriResponse
            {
                AuthUri = "https://auth.truelayer-sandbox.com/?response_type=code" +
                          $"&client_id={_options.ClientId}" +
                          $"&scope={request.Scope}" +
                          $"&redirect_uri={request.RedirectUri}" +
                          "&providers=uk-ob-all%20uk-oauth-all%20uk-cs-mock"
            };

            return await Task.FromResult(response);
        }

        public async Task<ExchangeCodeResponse> ExchangeCode(ExchangeCodeRequest request, CancellationToken cancellationToken)
        {
            const string path = "connect/token";
            
            var content = new FormUrlEncodedContent(new KeyValuePair<string?, string?>[]
            {
                new ("grant_type", "authorization_code"),
                new ("code", request.Code),
                new ("client_id", _options.ClientId),
                new ("client_secret", _options.ClientSecret),
                new ("redirect_uri", request.RedirectUri),
            });
            
            var apiResponse = await _apiClient.PostAsync<ExchangeCodeResponse>(GetRequestUri(path), content, null, cancellationToken);
            return apiResponse;
        }
        
        public async Task<AuthTokenResponse> GetPaymentToken(CancellationToken cancellationToken = default)
        {            
            const string path = "connect/token";
            
            var content = new FormUrlEncodedContent(new KeyValuePair<string?, string?>[]
            {
                new ("grant_type", "client_credentials"),
                new ("client_id", _options.ClientId),
                new ("client_secret", _options.ClientSecret),
                new ("scope", "payments"),
            });
            
            var apiResponse = await _apiClient.PostAsync<AuthTokenResponse>(GetRequestUri(path), content, null, cancellationToken);
            return apiResponse;
        }
        
        private Uri GetRequestUri(string path) => new (BaseUri, path);
    }
}
