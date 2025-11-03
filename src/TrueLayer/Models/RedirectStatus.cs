using System;

namespace TrueLayer.Models;

/// <summary>
/// Can the UI redirect the end user to a third-party page?
/// </summary>
/// <param name="ReturnUri">
/// During the authorization flow the end user might be redirected to another page(e.g.bank website, TrueLayer's Hosted Payment Page).
/// This URL determines where they will be redirected back once they completed the flow on the third-party's website.
/// This field is optional - if left unspecified, the end user will be redirected back to the default URI specified in your client settings in TrueLayer's Console.
/// return_uri, if specified, must be one of the allowed return_uris registered in TrueLayer's Console.
/// </param>
public record RedirectStatus(Uri ReturnUri);