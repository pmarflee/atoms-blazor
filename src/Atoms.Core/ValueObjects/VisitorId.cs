using Atoms.Core.Json.Converters;
using System.Text.Json.Serialization;

namespace Atoms.Core.ValueObjects;

[JsonConverter(typeof(VisitorIdJsonConverter))]
public record VisitorId(Guid Value);
