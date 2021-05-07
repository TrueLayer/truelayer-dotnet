using System.Collections.Generic;

namespace TrueLayer.Payments.Model.AuthFlowModel
{
    public class AuthFlowRequirement
    {
        /// <summary>
        /// A list of supported auth flow types.
        /// See <see cref="Constants.AuthFlowType"/>
        /// </summary>
        public List<string>? Types { get; set; }

        /// <summary>
        /// Will be set if redirect is supported.
        /// </summary>
        public RedirectFlowRequirement? Redirect { get; set; }

        /// <summary>
        /// Will be set if embedded is supported.
        /// </summary>
        public EmbeddedFlowRequirement? Embedded { get; set; }
    }

    public class EmbeddedFlowRequirement
    {
        /// <summary>
        /// A list of additional input field types that may be returned during the embedded authorisation flow
        /// See <see cref="Constants.AdditionalInputType"/>
        /// </summary>
        public List<string>? AdditionalInputTypes { get; set; }

        /// <summary>
        /// An optional dictionary of ID to additional inputs that must be collected from the end user to initiate a payment. 
        /// These inputs should be provided using the same IDs in the auth_flow.additional_inputs string dictionary on the initiation request.
        /// </summary>
        public AdditionalInputs? AdditionalInputs { get; set; }
    }

    public class RedirectFlowRequirement
    {
        /// <summary>
        /// (Optional) Indicates if auth_flow.psu_ip_address is required on the initiation request.
        /// </summary>
        public string? PsuIpAddressRequired { get; set; }

        /// <summary>
        /// (Optional) Indicates if auth_flow.data_access_token can be provided on the initiation request, 
        /// to maintain long lived consents across products (Data API and Payment API), 
        /// where the provider only allows a single active consent across both AIS and PIS services.
        /// </summary>
        public bool DataAccessTokenSupported { get; set; }

        /// <summary>
        /// (Optional) An optional dictionary of ID to additional inputs that must be collected 
        /// from the end user to initiate a payment. These inputs should be provided using the same IDs 
        /// in the auth_flow.additional_inputs string dictionary on the initiation request.
        /// </summary>
        public AdditionalInputs? AdditionalInputs { get; set; }
    }

    public class AdditionalInputs
    {
        /// <summary>
        /// Requires the end user to enter a string value.
        /// </summary>
        public TextInput? Text { get; set; }
        /// <summary>
        /// Displays an image to the end user and requires them to enter a string value.
        /// </summary>
        public TextWithImageInput? TextWithImage { get; set; }
        /// <summary>
        /// Provides a list of options for the end user to select from.
        /// </summary>
        public SelectInput? Select { get; set; }
    }

    public class TextInput
    {
        /// <summary>
        /// Value indicating the field type.
        /// See <see cref="Constants.AdditionalInputType"/>
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// Text to describe what the field is asking for (English)
        /// </summary>
        public string? DisplayText { get; set; }
        /// <summary>
        /// Text directly from the bank relating to the field
        /// </summary>
        public string? ProviderDescription { get; set; }
        /// <summary>
        /// If the input must be provided
        /// </summary>
        public bool Mandatory { get; set; }
        /// <summary>
        /// If the input is sensitive (should be masked in UI)
        /// </summary>
        public bool Sensitive { get; set; }
        /// <summary>
        /// The minimum length of the input text
        /// </summary>
        public int MinLength { get; set; }
        /// <summary>
        /// The maximum length of the input text
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// A regular expression the input text must match
        /// </summary>
        public string? Regex { get; set; }
        /// <summary>
        /// A well known format, to facilitate being able to show useful prompts to the user.
        /// See <see cref="Constants.StringFormat"/>
        /// </summary>
        public string? Format { get; set; }
    }

    public class TextWithImageInput : TextInput
    {
        /// <summary>
        /// An image to display to the end user (e.g. QR code).
        /// </summary>
        public Image? Image { get; set; }
    }

    public class Image
    {
        /// <summary>
        /// The image object can be of two types, base64 or uri.
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// This property will be filled if type is base64.
        /// </summary>
        public ImageBase64? Base64 { get; set; }
        /// <summary>
        /// This property will be filled if type is uri.
        /// </summary>
        public ImageUri? Uri { get; set; }
    }
    
    public class ImageBase64
    {
        /// <summary>
        /// Base64 data of the image.
        /// </summary>
        public string? Data { get; set; }
        /// <summary>
        /// Image media type: i.e image/png
        /// </summary>
        public string? MediaType { get; set; }
    }
    
    public class ImageUri
    {
        /// <summary>
        /// URI to a hosted copy of the image.
        /// </summary>
        public string? Uri { get; set; }
    }

    public class SelectInput
    {
        /// <summary>
        /// Value indicating the field type.
        /// See <see cref="Constants.AdditionalInputType"/>
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// Text to describe what the field is asking for (English)
        /// </summary>
        public string? DisplayText { get; set; }
        /// <summary>
        /// Text directly from the bank relating to the field
        /// </summary>
        public string? ProviderDescription { get; set; }
        /// <summary>
        /// If the input must be provided
        /// </summary>
        public bool Mandatory { get; set; }
        /// <summary>
        /// A list of id-display_text pairs to present to the end user as options (e.g. a drop down).
        /// </summary>
        public SelectOption[]? Options { get; set; }
    }

    public class SelectOption
    {
        /// <summary>
        /// Value to be provided on the initiation request, if the option is selected.
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// Text to describe the option.
        /// </summary>
        public string? DisplayText { get; set; }
    }
}
