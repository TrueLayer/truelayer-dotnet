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
        private readonly TruelayerOptions _options;
        internal readonly Uri BaseUri;
        
        public AuthClient(IApiClient apiClient, TruelayerOptions options)
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
            
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", request.Code),
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", request.RedirectUri),
            });
            
            var apiResponse = await _apiClient.PostAsync<ExchangeCodeResponse>(GetRequestUri(path), content, null, cancellationToken);
            return apiResponse;
        }
        
        public async Task<GetPaymentTokenResponse> GetPaymentToken(GetPaymentTokenRequest request, CancellationToken cancellationToken)
        {
            const string path = "connect/token";
            
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                new KeyValuePair<string, string>("scope", "payments"),
            });
            
            var apiResponse = await _apiClient.PostAsync<GetPaymentTokenResponse>(GetRequestUri(path), content, null, cancellationToken);
            return apiResponse;
        }
        
        private Uri GetRequestUri(string path) => new (BaseUri, path);
    }
}
