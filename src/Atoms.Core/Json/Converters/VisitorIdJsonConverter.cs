using System.Text.Json;
using System.Text.Json.Serialization;

namespace Atoms.Core.Json.Converters;

public class VisitorIdJsonConverter : JsonConverter<VisitorId>
{
    public override VisitorId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(reader.GetGuid());
    }

    public override void Write(Utf8JsonWriter writer, VisitorId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
