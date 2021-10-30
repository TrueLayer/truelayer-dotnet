using System;

namespace TrueLayer.Payments
{
    /// <summary>
    /// Options for the TrueLayer Payments API
    /// </summary>
    public class PaymentsOptions : ApiOptions
    {
        /// <summary>
        /// Gets or sets the public key used to sign outgoing payment requests
        /// </summary>
        public SigningKey? SigningKey { get; set; }
        
        /// <summary>
        /// Gets or sets the Hosted Payment Page URI. Defaults to Sandbox or Live depending on the value of <see cref="TrueLayerOptions.UseSandbox"/>
        /// </summary>
        public Uri? HppUri { get; set; }

        internal override void Validate()
        {
            base.Validate();

            if (SigningKey is null)
            {
                throw new ArgumentNullException(nameof(SigningKey));
            }

            if (string.IsNullOrWhiteSpace(SigningKey?.KeyId))
            {
                throw new ArgumentException("The signing key identifier is required", nameof(SigningKey.KeyId));
            }

            if (string.IsNullOrWhiteSpace(SigningKey?.PrivateKey))
            {
                throw new ArgumentException("The signing key is required", nameof(SigningKey.PrivateKey));
            }
        }
    }
}
