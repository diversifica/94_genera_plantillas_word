namespace TemplateGen.Core.Models;

using System.Text.Json.Serialization;

public record TemplateProfile(
    [property: JsonPropertyName("schema_version")] string SchemaVersion,
    [property: JsonPropertyName("metadata")] ProfileMetadata Metadata
);

public record ProfileMetadata(
    [property: JsonPropertyName("client_id")] string ClientId,
    [property: JsonPropertyName("client_name")] string ClientName,
    [property: JsonPropertyName("profile_version")] string ProfileVersion,
    [property: JsonPropertyName("description")] string? Description
);
