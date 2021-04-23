using System.Net.Http;

namespace TrueLayerSdk
{
    /// <summary>
    /// Interface for creating <see cref="System.Net.Http.HttpClient"/> instances.
    /// </summary>
    public interface IHttpClientFactory
    {
        /// <summary>
        /// Creates a <see cref="System.Net.Http.HttpClient"/> instance.
        /// </summary>
        /// <returns>An initialized instance.</returns>
        HttpClient CreateClient(); 
    }
}
