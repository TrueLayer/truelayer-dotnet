namespace TrueLayer
{
    using System;

    public class ApiOptions
    {
        public Uri? Uri { get; set; }
        
        public void Validate()
        {
            if (Uri is not { IsAbsoluteUri: true })
                throw new InvalidOperationException($"{nameof(Uri)} must be a valid and absolute uri.");
        }
    }
}
