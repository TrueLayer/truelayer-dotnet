using System;
using TrueLayer.Payments;

namespace TrueLayer
{
    /// <summary>
    /// Defines the options needed to configure the TrueLayer.com SDK for .NET that can be initialized
    /// using the Microsoft configuration framework.
    /// </summary>
    public class TrueLayerOptions
    {
        /// <summary>
        /// Gets or sets your TrueLayer client id.
        /// </summary>
        public string? ClientId { get; init; }
        
        /// <summary>
        /// Gets or sets your TrueLayer client secret.
        /// </summary>
        public string? ClientSecret { get; init; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to connect to the TrueLayer Sandbox. 
        /// </summary>
        public bool? UseSandbox { get; init; }

        public ApiOptions? Auth { get; set; }
        public PaymentsOptions? Payments { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ClientId)) throw new ArgumentException($"Your client id is required", nameof(ClientId));
            if (string.IsNullOrWhiteSpace(ClientSecret)) throw new ArgumentException($"Your API secret key is required", nameof(ClientSecret));
            
            Auth?.Validate();
            Payments?.Validate();
        }
    }
}
