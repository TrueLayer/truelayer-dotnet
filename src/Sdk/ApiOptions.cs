namespace TrueLayer
{
    using System;

    public class ApiOptions
    {
        public Uri? Uri { get; set; }
        
        public virtual void Validate()
        {
            if (Uri is { IsAbsoluteUri: false })
                throw new InvalidOperationException($"{nameof(Uri)} must be a valid and absolute uri.");
        }
    }
}
