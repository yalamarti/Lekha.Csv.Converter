using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lekha.Csv.Converter
{
    /// <summary>
    /// A JSON converter for converting a list of items, that makes use of a JsonConverted defined for the item.
    /// </summary>
    /// <typeparam name="ItemConverter">JsonConverter type that represents the list item</typeparam>
    /// <typeparam name="T">Type of the item in the list</typeparam>
    public class ItemListConverter<ItemConverter, T> : JsonConverter<List<T>>  where T : class where ItemConverter : JsonConverter<T>, new()
    {
        readonly ItemConverter itemConverter = new();

        /// <summary>
        /// Reads json from the specified JSON reader and converts into list of specified T items
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
            }

            var dictionary = new List<T>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return dictionary;
                }

                dictionary.Add(ExtractValue(ref reader, options));
            }

            return dictionary;
        }

        private T ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.StartObject => itemConverter.Read(ref reader, null, options),
                _ => throw new JsonException($"'{reader.TokenType}' is not supported"),
            };
        }

        /// <summary>
        /// Writes specified list of items as json the specified JSON writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="values"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, List<T> values, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var value in values)
            {
                itemConverter.Write(writer, value, options);
            }

            writer.WriteEndArray();
        }
    }
}
