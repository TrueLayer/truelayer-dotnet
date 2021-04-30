namespace TrueLayerSdk.Data
{
    /// <summary>
    /// Default implementation of <see cref="IDataClient"/>.
    /// </summary>
    internal class DataClient : IDataClient
    {
        private readonly IApiClient _apiClient;
        private readonly TruelayerConfiguration _configuration;

        public DataClient(IApiClient apiClient, TruelayerConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
        }
    }
}
