using System;
using System.Text.Json;

namespace TrueLayerSdk.Common.Serialization
{
    public class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Creates a new <see cref="JsonSerializer"/> that allows customization of the underlying
        /// System.Text.Json serializer settings.
        /// </summary>
        /// <param name="configureOptions">An action to be run against the System.Text.Json serializer options.</param>
        public JsonSerializer(Action<JsonSerializerOptions> configureOptions = null)
        {
            _serializerOptions = CreateSerializerSettings(configureOptions);
        }
        
        /// <summary>
        /// Serializes the provided <paramref name="input"/> to JSON.
        /// </summary>
        /// <param name="input">The input to serialize.</param>
        /// <returns>The input serialized as JSON.</returns>
        public string Serialize(object input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return System.Text.Json.JsonSerializer.Serialize(input, _serializerOptions);
        }

        /// <summary>
        /// Deserializes the provided JSON <paramref name="input"/> to the specified <paramref name="objectType"/>.
        /// </summary>
        /// <param name="input">The JSON input to deserialize.</param>
        /// <param name="objectType">The object type to deserialize to.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(string input, Type objectType)
        {
            return System.Text.Json.JsonSerializer.Deserialize(input, objectType, _serializerOptions);
        }
        
        private static JsonSerializerOptions CreateSerializerSettings(Action<JsonSerializerOptions> configureOptions)
        {
            var settings = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
                // Converters = new[] { new StringEnumConverter() }
            };

            configureOptions?.Invoke(settings);

            return settings;
        }
    }
}
