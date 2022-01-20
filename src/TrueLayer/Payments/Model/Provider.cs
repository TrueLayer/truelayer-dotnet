namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Payment action types
    /// </summary>
    public static class Provider
    {
        /// <summary>
        /// Represents a selection action
        /// </summary>
        public record SelectionAction : IDiscriminated
        {
            /// <summary>
            /// Creates a new <see cref="SelectionAction"/> instance
            /// </summary>
            /// <param name="providerFilter">Filter options for providers</param>
            public SelectionAction(ProviderFilter providerFilter)
            {
                ProviderFilter = providerFilter;
            }

            /// <summary>
            /// Gets the provider filter
            /// </summary>
            public ProviderFilter ProviderFilter { get; }


            /// <summary>
            /// Gets the selection action type
            /// </summary>
            public string Type => "selection_action";
        }
    }
}
