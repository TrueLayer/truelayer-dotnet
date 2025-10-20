using System;
using OneOf;

namespace TrueLayer.Payments.Model.AuthorizationFlow;

using ProviderSelectionUnion = OneOf<CreateProvider.UserSelected, CreateProvider.Preselected>;
using SchemeSelectionUnion = OneOf<
    SchemeSelection.UserSelected,
    SchemeSelection.Preselected,
    SchemeSelection.InstantOnly,
    SchemeSelection.InstantPreferred
>;

/// <summary>
/// Can the UI redirect the end user to a third-party page? Configuration options are available to constrain if TrueLayer's Hosted Payment Page should be leveraged.
/// </summary>
/// <param name="ReturnUri">During the authorization flow the end user might be redirected to another page (e.g.bank website, TrueLayer's Hosted Payment Page). This URL determines where they will be redirected back once they completed the flow on the third-party's website. return_uri must be one of the allowed return_uris registered in TrueLayer's Console.</param>
/// <param name="DirectReturnUri">Only applicable if you're regulated and have a direct return URI registered with UK providers.
/// If you're regulated, you can specify a direct_return_uri to attempt the authorization flow via a direct redirect from the provider authorization page to your page without going via TrueLayer.
/// We recommend that your return_uri is a URI that can handle a non-direct return scenario. This ensures that if the direct_return_uri isn't registered with the user's chosen provider, the payment can still be authorized through a Truelayer domain.</param>
public record Redirect(Uri ReturnUri, Uri? DirectReturnUri = null);

public record UserAccountSelection();

/// <summary>
/// Start the authorization flow for a mandate.
/// </summary>
/// <param name="ProviderSelection">Can the UI render a provider selection screen?</param>
/// <param name="SchemeSelection">Can your UI render a scheme selection screen?
/// For payments where you set scheme_selection as user_selected, your UI must be able to render a screen where the user can select their payments scheme.
/// This field is required for payments with user_selected scheme selection. For other scheme selection types, it's optional</param>
/// <param name="Form">Can your UI render form inputs for the user to interact with?
/// Some providers require additional inputs, such as the remitter name and account details, to be provided before or during payment authorization.
/// To facilitate this, the API may return a form action as part of the authorization flow, which means your UI must be able to collect the required inputs.
/// This parameter states whether your UI supports the form action. If you omit this parameter, the API returns only providers that don't require additional inputs.
/// If the provider has been preselected and requires additional inputs, this field is required.</param>
/// <param name="Redirect">Can the UI redirect the end user to a third-party page? Configuration options are available to constrain if TrueLayer's Hosted Payment Page should be leveraged.</param>
/// <param name="Consent">Can the UI capture the user's consent? This field declares whether the UI supports the consent action, which is used to explicitly capture the end user's consent for initiating the payment. If it is omitted, the flow will continue without a consent action.</param>
/// <param name="UserAccountSelection">Can your UI render a user account selection screen?
/// If the user has previously consented to saving their bank account details with TrueLayer, they can choose from their saved accounts to speed up following payments.
/// This field states whether your UI can render a selection screen for these saved accounts. If you omit this, the user isn't presented with this option.</param>
/// <param name="Retry">The retry opt-in option for the authorization flow</param>
public record StartAuthorizationFlowRequest(
    ProviderSelectionUnion? ProviderSelection = null,
    SchemeSelectionUnion? SchemeSelection = null,
    Redirect? Redirect = null,
    Form? Form = null,
    Consent? Consent = null,
    UserAccountSelection? UserAccountSelection = null,
    Retry.BaseRetry? Retry = null);

/// <summary>
/// Contains the information regarding the configuration of the authorization flow.
/// </summary>
/// <param name="ProviderSelection">The configured provider selection option</param>
/// <param name="SchemeSelection">The configured scheme selection option</param>
/// <param name="Form">The configured form option</param>
/// <param name="Redirect">The configured redirect option</param>
/// <param name="Consent">The configured consent option</param>
/// <param name="UserAccountSelection">The configured user account selection option</param>
/// <param name="Retry">The configured retry option</param>
public record Configuration(
    ProviderSelectionUnion? ProviderSelection = null,
    SchemeSelectionUnion? SchemeSelection = null,
    Redirect? Redirect = null,
    Form? Form = null,
    Consent? Consent = null,
    UserAccountSelection? UserAccountSelection = null,
    Retry.BaseRetry? Retry = null);