using Newtonsoft.Json;

namespace PixelByProxy.Asus.Router.Json
{
    internal class BoolConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteValue(bool.TryParse(value?.ToString(), out var parsed) ? parsed ? 1 : 0 : 0);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return int.TryParse(reader.Value?.ToString(), out var parsed) && parsed == 1;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool) || objectType == typeof(bool?);
        }
    }
}
