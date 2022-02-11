using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Provider types
    /// </summary>
    public static class Provider
    {
        /// <summary>
        /// Represents provider options for 'submit provider selection' action
        /// </summary>
        [JsonDiscriminator("user_selected")]
        public record UserSelected : IDiscriminated
        {
            /// <summary>
            /// Gets the provider type
            /// </summary>
            public string Type => "user_selected";

            /// <summary>
            /// Gets or sets the filter used to determine the banks that should be displayed on the bank selection screen
            /// </summary>
            public ProviderFilter? Filter { get; init; }
        }
    }
}
