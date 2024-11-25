using System.ComponentModel.DataAnnotations;
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
        /// Gets or inits your TrueLayer client id.
        /// </summary>
        public string? ClientId { get; init; }

        /// <summary>
        /// Gets or inits your TrueLayer client secret.
        /// </summary>
        public string? ClientSecret { get; init; }

        /// <summary>
        /// Gets or inits a value indicating whether to connect to the TrueLayer Sandbox.
        /// </summary>
        public bool? UseSandbox { get; init; }

        public ApiOptions? Auth { get; set; }
        public PaymentsOptions? Payments { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ClientId))
            {
                throw new ValidationException("Your client id is required");
            }

            if (string.IsNullOrWhiteSpace(ClientSecret))
            {
                throw new ValidationException("Your API secret key is required");
            }

            // Only call validate on the Options that must be present for any use of the library
            // otherwise each API client should be responsible for validating it's options internally
            Auth?.Validate();
        }
    }
}
