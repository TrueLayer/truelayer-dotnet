namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Represents a payment user.
    /// </summary>
    public record PaymentUserRequest
    {
        /// <summary>
        /// Creates a payment user.
        /// </summary>
        /// <param name="name">The user's name.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="phone">The user's phone number.</param>
        public PaymentUserRequest(string? name = null, string? email = null, string? phone = null)
        {
            Name = name.NotEmptyOrWhiteSpace(nameof(name));
            Email = email.NotEmptyOrWhiteSpace(nameof(email));
            Phone = phone.NotEmptyOrWhiteSpace(nameof(phone));
        }

        /// <summary>
        /// Creates a payment user using an existing identifier.
        /// </summary>
        /// <param name="id">The user's identifier</param>
        /// <param name="name">The user's name</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="phone">The user's phone number.</param>
        public PaymentUserRequest(string id, string? name = null, string? email = null, string? phone = null)
        {
            Id = id.NotNullOrWhiteSpace(nameof(id));
            Name = name.NotEmptyOrWhiteSpace(nameof(name));
            Email = email.NotEmptyOrWhiteSpace(nameof(email));
            Phone = phone.NotEmptyOrWhiteSpace(nameof(phone));
        }

        /// <summary>
        /// Gets the user's identifier.
        /// </summary>
        public string? Id { get; }

        /// <summary>
        /// Gets the user's name.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        public string? Email { get; }

        /// <summary>
        /// Gets the user's phone number.
        /// </summary>
        public string? Phone { get; }
    }
}
