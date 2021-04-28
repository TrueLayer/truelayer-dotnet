using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrueLayerSdk.Auth.Models;

namespace TrueLayerSdk.Auth
{
    /// <summary>
    /// Default implementation of <see cref="IAuthClient"/>.
    /// </summary>
    internal class AuthClient : IAuthClient
    {
        private readonly IApiClient _apiClient;
        private readonly TruelayerConfiguration _configuration;

        public AuthClient(IApiClient apiClient, TruelayerConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
        }

        public async Task<GetAuthUriResponse> GetAuthUri(GetAuthUriRequest request)
        {
            var response = new GetAuthUriResponse
            {
                AuthUri = "https://auth.truelayer-sandbox.com/?response_type=code" +
                          $"&client_id={_configuration.ClientId}" +
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
                new KeyValuePair<string, string>("client_id", _configuration.ClientId),
                new KeyValuePair<string, string>("client_secret", _configuration.ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", request.RedirectUri),
            });
            
            var apiResponse = await _apiClient.PostAsync<ExchangeCodeResponse>(GetRequestUri(path), content, cancellationToken);
            return apiResponse;
        }
        
        public async Task<GetPaymentTokenResponse> GetPaymentToken(GetPaymentTokenRequest request, CancellationToken cancellationToken)
        {
            const string path = "connect/token";
            
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _configuration.ClientId),
                new KeyValuePair<string, string>("client_secret", _configuration.ClientSecret),
                new KeyValuePair<string, string>("scope", "payments"),
            });
            
            var apiResponse = await _apiClient.PostAsync<GetPaymentTokenResponse>(GetRequestUri(path), content, cancellationToken);
            return apiResponse;
        }
        
        private Uri GetRequestUri(string path) => new (_configuration.AuthUri, path);
    }
}
