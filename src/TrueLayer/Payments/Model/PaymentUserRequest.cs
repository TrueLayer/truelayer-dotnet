using System;

namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Represents a payment user.
    /// </summary>
    public record PaymentUserRequest
    {
        private PaymentUserRequest(string? id, string? name, string? email, string? phone)
        {
            Id = id;
            Name = name;
            Email = email;
            Phone = phone;
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

        /// <summary>
        /// Creates a new payment user for the case where user details must be provided with a payment.
        /// </summary>
        /// <param name="name">The user's name.</param>
        /// <param name="email">The user's email address. Either an email address or <param ref="phone"/> must be provided.</param>
        /// <param name="phone">The user's phone number. Either a phone number or <param ref="email"/> must be provided.</param>
        public static PaymentUserRequest New(string name, string? email = null, string? phone = null)
        {
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("User email or phone must be provided");
            }

            return new(
                null,
                name.NotNullOrWhiteSpace(nameof(name)),
                email.NotEmptyOrWhiteSpace(nameof(email)),
                phone.NotEmptyOrWhiteSpace(nameof(phone)));
        }

        /// <summary>
        /// Creates a new payment user for the case where user details are not mandatory to provide with a payment.
        /// This is the case if the client is regulated and has been approved to bypass providing these details.
        /// </summary>
        /// <param name="name">The user's name.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="phone">The user's phone number.</param>
        public static PaymentUserRequest? NewOptional(string? name = null, string? email = null, string? phone = null)
        {
            if (name is null && email is null & phone is null)
            {
                return null;
            }
            return new(null, name, email, phone);
        }

        /// <summary>
        /// Creates an existing payment user for the case where user details must be provided with a payment.
        /// </summary>
        /// <param name="id">The user's identifier</param>
        /// <param name="name">The user's name</param>
        /// <param name="email">The user's email address. Either an email address or <param ref="phone"/> must be provided.</param>
        /// <param name="phone">The user's phone number. Either a phone number or <param ref="email"/> must be provided.</param>
        public static PaymentUserRequest Existing(string id, string name, string? email = null, string? phone = null)
        {
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("User email or phone must be provided");
            }

            return new(
                id.NotNullOrWhiteSpace(nameof(id)),
                name.NotNullOrWhiteSpace(nameof(name)),
                email.NotEmptyOrWhiteSpace(nameof(email)),
                phone.NotEmptyOrWhiteSpace(nameof(phone)));
        }

        /// <summary>
        /// Creates an existing payment user for the case where user details are not mandatory to provide with a payment.
        /// This is the case if the client is regulated and has been approved to bypass providing these details.
        /// </summary>
        /// <param name="id">The user's identifier</param>
        /// <param name="name">The user's name</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="phone">The user's phone number.</param>
        public static PaymentUserRequest ExistingOptional(
            string id,
            string? name = null,
            string? email = null,
            string? phone = null)
            => new(id, name, email, phone);
    }
}
