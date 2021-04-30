using System;

namespace TrueLayer
{
    /// <summary>
    /// Base class for exceptions thrown by the Truelayer.com SDK for .NET.
    /// </summary>
    public class TrueLayerException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="TrueLayerException"/> instance with the provided message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <returns></returns>
        public TrueLayerException(string message) : base(message) { }
    }
}
