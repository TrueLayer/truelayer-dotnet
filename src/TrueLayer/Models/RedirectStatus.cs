using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OneOf.Types;
using TrueLayer.Serialization;

namespace TrueLayer.Models
{
    /// <summary>
    /// Can the UI redirect the end user to a third-party page?
    /// </summary>
    public static class RedirectStatus
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Type { Supported = 0, NotSupported = 1 }

        /// <summary>
        /// Supported redirect status object
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="ReturnUri">
        /// During the authorization flow the end user might be redirected to another page(e.g.bank website, TrueLayer's Hosted Payment Page).
        /// This URL determines where they will be redirected back once they completed the flow on the third-party's website.
        /// This field is optional - if left unspecified, the end user will be redirected back to the default URI specified in your client settings in TrueLayer's Console.
        /// return_uri, if specified, must be one of the allowed return_uris registered in TrueLayer's Console.
        /// </param>
        [JsonDiscriminator("supported")]
        public record SupportedRedirectStatus(Type Status, Uri ReturnUri);

        /// <summary>
        /// Not supported redirect status object.
        /// </summary>
        /// <param name="Status">not_supported</param>
        [JsonDiscriminator("not_supported")]
        public record NotSupportedRedirectStatus(Type Status);
    }
}
