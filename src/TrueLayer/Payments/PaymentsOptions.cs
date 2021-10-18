using System;

namespace TrueLayer.Payments
{
    public class PaymentsOptions : ApiOptions
    {
        public SigningKey? SigningKey { get; set; }

        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrWhiteSpace(SigningKey?.KeyId))
            {
                throw new ArgumentNullException(nameof(SigningKey.KeyId), "The signing key identifier is required");
            }

            if (string.IsNullOrWhiteSpace(SigningKey?.Certificate))
            {
                throw new ArgumentNullException(nameof(SigningKey.Certificate), "The signing certificate is required");
            }
        }
    }
}
