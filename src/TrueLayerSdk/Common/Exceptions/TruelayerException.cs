using System;

namespace TrueLayerSdk.Common.Exceptions
{
    /// <summary>
    /// Base class for exceptions thrown by the Truelayer.com SDK for .NET.
    /// </summary>
    public class TruelayerException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="TruelayerException"/> instance with the provided message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <returns></returns>
        public TruelayerException(string message) : base(message) { }
    }
}
