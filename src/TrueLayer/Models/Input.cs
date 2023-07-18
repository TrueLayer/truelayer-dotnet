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
        public record DisplayText(string Key, [property: JsonPropertyName("default")]string DefaultValue);

        public record Regex([property: JsonPropertyName("regex")] string Value, DisplayText Message);

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Format { Any = 0, Numerical = 1, Alphabetical = 2, Alphanumerical = 3, Email = 4, SortCode = 5, AccountNumber = 6, Iban = 7 };

        public abstract record InputBase(
            string Type,
            string Id,
            bool Mandatory,
            DisplayText DisplayText,
            string Description) : IDiscriminated;

        [JsonDiscriminator("text")]
        public abstract record TextBase(
            string Type,
            string Id,
            bool Mandatory,
            DisplayText DisplayText,
            string Description,
            Format Format,
            bool Sensitive,
            int MinLength,
            int MaxLength,
            List<Regex> Regexes)
            : InputBase(
                Type,
                Id,
                Mandatory,
                DisplayText,
                Description);

        [JsonDiscriminator("text")]
        public record Text(
            string Type,
            string Id,
            bool Mandatory,
            DisplayText DisplayText,
            string Description,
            Format Format,
            bool Sensitive,
            int MinLength,
            int MaxLength,
            List<Regex> Regexes)
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

        [JsonDiscriminator("text_with_image")]
        public record TextWithImage(
            string Type,
            string Id,
            bool Mandatory,
            DisplayText DisplayText,
            string Description,
            Format Format,
            bool Sensitive,
            int MinLength,
            int MaxLength,
            List<Regex> Regexes,
            ImageUnion Image)
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

        [JsonDiscriminator("select")]
        public record Select(
            string Type,
            string Id,
            bool Mandatory,
            DisplayText DisplayText,
            string Description,
            List<Option> Options)
            : InputBase(
                Type,
                Id,
                Mandatory,
                DisplayText,
                Description);
    }
}
