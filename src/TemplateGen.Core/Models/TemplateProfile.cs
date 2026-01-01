namespace TemplateGen.Core.Models;

using System.Text.Json.Serialization;

public record TemplateProfile(
    [property: JsonPropertyName("schema_version")] string SchemaVersion,
    [property: JsonPropertyName("metadata")] ProfileMetadata Metadata,
    [property: JsonPropertyName("document")] DocumentSettings? Document,
    [property: JsonPropertyName("styles")] StylesConfig? Styles,
    [property: JsonPropertyName("numbering")] NumberingConfig? Numbering
);

public record ProfileMetadata(
    [property: JsonPropertyName("client_id")] string ClientId,
    [property: JsonPropertyName("client_name")] string ClientName,
    [property: JsonPropertyName("profile_version")] string ProfileVersion,
    [property: JsonPropertyName("description")] string? Description
);
