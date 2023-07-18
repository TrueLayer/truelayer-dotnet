using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace TrueLayer.Models
{
    using ImageUnion = OneOf<Image.Uri, Image.Base64>;

    public static class Input
    {
        /// <summary>
        /// Display Text object.
        /// </summary>
        /// <param name="Key">A key that is intended to be used as a translation key to look up and render localised text. If a key is not provided, it indicates that this value cannot be localised and that the default value should always be preferred.</param>
        /// <param name="DefaultValue">A value that can be used as a default to show to the user, if the key cannot be used to look up a relevant value.</param>
        public record DisplayText(string Key, [property: JsonPropertyName("default")]string DefaultValue);

        /// <summary>
        /// A regex to validate the input against.
        /// </summary>
        /// <param name="Value">A regular expression for defining the accepted format of the text input.</param>
        /// <param name="Message">A validation message to show if the text input does not match the corresponding regex.</param>
        public record Regex([property: JsonPropertyName("regex")] string Value, DisplayText Message);

        /// <summary>
        /// The type of text input that this represents.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Format { Any = 0, Numerical = 1, Alphabetical = 2, Alphanumerical = 3, Email = 4, SortCode = 5, AccountNumber = 6, Iban = 7 };

        public abstract record InputBase(
            string Type,
            string Id,
            bool Mandatory,
            DisplayText DisplayText,
            string? Description) : IDiscriminated;

        public abstract record TextBase(
            string Type,
            string Id,
            bool Mandatory,
            DisplayText DisplayText,
            string? Description,
            Format Format,
            bool Sensitive,
            int MinLength,
            int MaxLength,
            List<Regex>? Regexes = null)
            : InputBase(
                Type,
                Id,
                Mandatory,
                DisplayText,
                Description);

        /// <summary>
        /// Text input object
        /// </summary>
        /// <param name="Type">text</param>
        /// <param name="Id">The identifier for the additional input, to be used when submitting the user's value back to the API.</param>
        /// <param name="Mandatory">Whether or not a value for this input must be provided when the form is submitted.</param>
        /// <param name="DisplayText">The text to be used as a label for the input, to be rendered in the UI somewhere alongside the input itself.</param>
        /// <param name="Description">Additional text that provides more information about the input and the value that the PSU is expected to provide.</param>
        /// <param name="Format">The type of text input that this represents.</param>
        /// <param name="Sensitive">Whether or not the input contains sensitive information (e.g. a password). This can be used to render a field in the UI that masks input.</param>
        /// <param name="MinLength">The minimum length (inclusive) of the text input value.</param>
        /// <param name="MaxLength">The maximum length (inclusive) of the text input value.</param>
        /// <param name="Regexes">A collection of regexes to validate the input against.</param>
        [JsonDiscriminator("text")]
        public record Text(
            string Type,
            string Id,
            bool Mandatory,
            DisplayText DisplayText,
            Format Format,
            bool Sensitive,
            int MinLength,
            int MaxLength,
            List<Regex>? Regexes,
            string? Description = null)
            : TextBase(
                Type,
                Id,
                Mandatory,
                DisplayText,
                Description,
                Format,
                Sensitive,
                MinLength,
                MaxLength,
                Regexes);


        /// <summary>
        /// Text input object with image
        /// </summary>
        /// <param name="Type">text_with_image</param>
        /// <param name="Id">The identifier for the additional input, to be used when submitting the user's value back to the API.</param>
        /// <param name="Mandatory">Whether or not a value for this input must be provided when the form is submitted.</param>
        /// <param name="DisplayText">The text to be used as a label for the input, to be rendered in the UI somewhere alongside the input itself.</param>
        /// <param name="Description">Additional text that provides more information about the input and the value that the PSU is expected to provide.</param>
        /// <param name="Format">The type of text input that this represents.</param>
        /// <param name="Sensitive">Whether or not the input contains sensitive information (e.g. a password). This can be used to render a field in the UI that masks input.</param>
        /// <param name="MinLength">The minimum length (inclusive) of the text input value.</param>
        /// <param name="MaxLength">The maximum length (inclusive) of the text input value.</param>
        /// <param name="Regexes">A collection of regexes to validate the input against.</param>
        /// <param name="Image"></param>
        [JsonDiscriminator("text_with_image")]
        public record TextWithImage(
            string Type,
            string Id,
            bool Mandatory,
            DisplayText DisplayText,
            Format Format,
            bool Sensitive,
            int MinLength,
            int MaxLength,
            List<Regex> Regexes,
            ImageUnion Image,
            string? Description = null)
            : TextBase(
                Type,
                Id,
                Mandatory,
                DisplayText,
                Description,
                Format,
                Sensitive,
                MinLength,
                MaxLength,
                Regexes);

        public record Option (string Id, DisplayText DisplayText);

        /// <summary>
        /// Select input object.
        /// </summary>
        /// <param name="Type">select</param>
        /// <param name="Id">The identifier for the additional input, to be used when submitting the user's value back to the API.</param>
        /// <param name="Mandatory">Whether or not a value for this input must be provided when the form is submitted.</param>
        /// <param name="DisplayText">The text to be used as a label for the input, to be rendered in the UI somewhere alongside the input itself.</param>
        /// <param name="Description">Additional text that provides more information about the input and the value that the PSU is expected to provide.</param>
        /// <param name="Options">Exhaustive list of values to select from.</param>
        [JsonDiscriminator("select")]
        public record Select(
            string Type,
            string Id,
            bool Mandatory,
            DisplayText DisplayText,
            List<Option> Options,
            string? Description = null)
            : InputBase(
                Type,
                Id,
                Mandatory,
                DisplayText,
                Description);
    }
}
