using System.ComponentModel.DataAnnotations;

namespace TrueLayer.Payouts
{
    /// <summary>
    /// Options for the TrueLayer Payments API
    /// </summary>
    public class PayoutsOptions : ApiOptions
    {
        /// <summary>
        /// Gets or sets the public key used to sign outgoing payment requests
        /// </summary>
        public SigningKey? SigningKey { get; set; }

        internal override void Validate()
        {
            base.Validate();

            if (SigningKey is null)
            {
                throw new ValidationException("The signing key is required");
            }

            if (string.IsNullOrWhiteSpace(SigningKey?.KeyId))
            {
                throw new ValidationException("The signing key identifier is required");
            }

            if (string.IsNullOrWhiteSpace(SigningKey?.PrivateKey))
            {
                throw new ValidationException("The signing key is required");
            }
        }
    }
}
