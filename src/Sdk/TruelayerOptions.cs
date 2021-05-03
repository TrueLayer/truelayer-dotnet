using System;

namespace TrueLayer
{
    /// <summary>
    /// Defines the options needed to configure the Truelayer.com SDK for .NET that can be initialized
    /// using the Microsoft configuration framework.
    /// </summary>
    public class TruelayerOptions
    {
        /// <summary>
        /// Gets or sets your Truelayer client id.
        /// </summary>
        public string? ClientId { get; set; }
        
        /// <summary>
        /// Gets or sets your Truelayer client secret.
        /// </summary>
        public string? ClientSecret { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to connect to the Truelayer Sandbox. 
        /// </summary>
        public bool? UseSandbox { get; init; }

        public ApiOptions? Auth { get; set; }
        public ApiOptions? Payments { get; set; }
        public ApiOptions? Data { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ClientId)) throw new ArgumentException($"Your client id is required", nameof(ClientId));
            if (string.IsNullOrWhiteSpace(ClientSecret)) throw new ArgumentException($"Your API secret key is required", nameof(ClientSecret));
            
            Auth?.Validate();
            Payments?.Validate();
            Data?.Validate();
        }
    }
}
