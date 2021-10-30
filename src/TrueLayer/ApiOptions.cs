namespace TrueLayer
{
    using System;

    /// <summary>
    /// Represents default options for an API resource
    /// </summary>
    public class ApiOptions
    {
        /// <summary>
        /// Gets or sets the API base URI
        /// </summary>
        public Uri? Uri { get; set; }

        internal virtual void Validate()
        {
            if (Uri is { IsAbsoluteUri: false })
                throw new InvalidOperationException($"{nameof(Uri)} must be a valid and absolute uri.");
        }
    }
}
