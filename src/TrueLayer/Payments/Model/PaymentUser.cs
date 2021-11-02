namespace TrueLayer.Payments.Model
{
    using System;

    /// <summary>
    /// Payment user types
    /// </summary>
    public static class PaymentUser
    {
        /// <summary>
        /// Represents a new user
        /// </summary>
        public record NewUser : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="NewUser"/> instance
            /// </summary>
            /// <param name="name">The user's name</param>
            /// <param name="email">The user's email address. Either an email address or <param ref="phone"/> must be provided.</param>
            /// <param name="phone">The user's phone number. Either a phone number or <param ref="email"/> must be provided.</param>
            public NewUser(string name, string? email = null, string? phone = null)
            {
                Name = name.NotNullOrWhiteSpace(nameof(name));                
                
                if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
                {
                    throw new ArgumentException("User email or phone must be provided");
                }
                
                Email = email;
                Phone = phone;
            }

            /// <summary>
            /// Gets the user's name
            /// </summary>
            public string Name { get; }
            
            /// <summary>
            /// Gets the user's email address
            /// </summary>
            public string? Email { get; }
            
            /// <summary>
            /// Gets the user's phone number
            /// </summary>
            public string? Phone { get; }

            /// <summary>
            /// Gets the user type
            /// </summary>
            public string Type => "new";
        }

        /// <summary>
        /// Represents an existing user
        /// </summary>
        public record ExistingUser : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="ExistingUser"/> instance
            /// </summary>
            /// <param name="id">The user identifier</param>
            public ExistingUser(string id)
            {
                Id = id.NotNullOrWhiteSpace(nameof(id));
            }

            /// <summary>
            /// Gets the user identifier
            /// </summary>
            public string Id { get; }
            
            /// <summary>
            /// Gets the user type
            /// </summary>
            public string Type => "existing";
        }
    }
}
