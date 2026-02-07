using Atoms.Core.Json.Converters;
using System.Text.Json.Serialization;

namespace Atoms.Core.ValueObjects;

[JsonConverter(typeof(VisitorIdJsonConverter))]
public record struct VisitorId(Guid Value)
{
    //public static implicit operator Guid(VisitorId visitorId) => visitorId.Value;
    public static implicit operator VisitorId(Guid id) => new(id);
}
