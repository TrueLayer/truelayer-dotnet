using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;

namespace TrueLayer.Models
{
    /// <summary>
    /// Can the UI redirect the end user to a third-party page? Configuration options are available to constrain if TrueLayer's Hosted Payment Page should be leveraged.
    /// </summary>
    /// <param name="ReturnUri">During the authorization flow the end user might be redirected to another page (e.g.bank website, TrueLayer's Hosted Payment Page). This URL determines where they will be redirected back once they completed the flow on the third-party's website. return_uri must be one of the allowed return_uris registered in TrueLayer's Console.</param>
    public record Redirect(Uri ReturnUri);

    /// <summary>
    /// Can the UI capture the user's consent? This field declares whether the UI supports the consent action, which is used to explicitly capture the end user's consent for initiating the payment. If it is omitted, the flow will continue without a consent action.
    /// </summary>
    public record Consent();

    /// <summary>
    /// Can the UI render a provider selection screen?
    /// </summary>
    public record ProviderSelectionRequest();

    /// <summary>
    /// Start the authorization flow for a mandate.
    /// </summary>
    /// <param name="ProviderSelection">Can the UI render a provider selection screen?</param>
    /// <param name="Redirect">Can the UI redirect the end user to a third-party page? Configuration options are available to constrain if TrueLayer's Hosted Payment Page should be leveraged.</param>
    /// <param name="Consent">Can the UI capture the user's consent? This field declares whether the UI supports the consent action, which is used to explicitly capture the end user's consent for initiating the payment. If it is omitted, the flow will continue without a consent action.</param>
    public record StartAuthorizationFlowRequest(
        ProviderSelectionRequest ProviderSelection,
        Redirect Redirect,
        Consent? Consent = null);
}
