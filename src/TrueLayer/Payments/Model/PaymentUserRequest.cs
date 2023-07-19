using System;
using System.Text.Json.Serialization;
using TrueLayer.Common;
using TrueLayer.Serialization;

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
        /// <param name="dateOfBirth">The user's date of birth</param>
        /// <param name="address">The user's physical address</param>
        public PaymentUserRequest(
            string? name = null,
            string? email = null,
            string? phone = null,
            DateTime? dateOfBirth = null,
            Address? address = null)
        {
            Address = address;
            Name = name.NotEmptyOrWhiteSpace(nameof(name));
            Email = email.NotEmptyOrWhiteSpace(nameof(email));
            Phone = phone.NotEmptyOrWhiteSpace(nameof(phone));
            DateOfBirth = dateOfBirth?.Date;
            Address = address;
        }

        /// <summary>
        /// Creates a payment user using an existing identifier.
        /// </summary>
        /// <param name="id">The user's identifier</param>
        /// <param name="name">The user's name</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="phone">The user's phone number.</param>
        /// <param name="dateOfBirth">The user's date of birth</param>
        /// <param name="address">The user's physical address</param>
        public PaymentUserRequest(
            string id,
            string? name = null,
            string? email = null,
            string? phone = null,
            DateTime? dateOfBirth = null,
            Address? address = null)
        {
            Id = id.NotNullOrWhiteSpace(nameof(id));
            Name = name.NotEmptyOrWhiteSpace(nameof(name));
            Email = email.NotEmptyOrWhiteSpace(nameof(email));
            Phone = phone.NotEmptyOrWhiteSpace(nameof(phone));
            DateOfBirth = dateOfBirth?.Date;
            Address = address;
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
        /// Gets the user's date of birth.
        /// </summary>
        [JsonConverter(typeof(DateTimeDateOnlyJsonConverter))]
        public DateTime? DateOfBirth { get; }
        
        /// <summary>
        /// Gets the user's physical address
        /// </summary>
        public Address? Address { get; }
    }
}
