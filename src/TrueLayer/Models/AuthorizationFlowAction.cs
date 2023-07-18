using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Models.Input;

namespace TrueLayer.Models
{
    using InputUnion = OneOf<Input.Text, Input.TextWithImage, Input.Select>;

    public static class AuthorizationFlowAction
    {
        [JsonDiscriminator("provider_selection")]
        public record ProviderSelection(string Type, List<Provider> Providers) : IDiscriminated;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SubsequentActionHint { Redirect = 0, Form = 1 };

        [JsonDiscriminator("consent")]
        public record Consent(string Type, SubsequentActionHint SubsequentActionHint) : IDiscriminated;

        [JsonDiscriminator("form")]
        public record Form(string Type, List<InputUnion> Inputs) : IDiscriminated;

        [JsonDiscriminator("wait")]
        public record WaitForOutcome(string Type, DisplayText DisplayMessage) : IDiscriminated;

        [JsonDiscriminator("redirect")]
        public record Redirect(string Type, Uri Uri) : IDiscriminated;
    }
}
