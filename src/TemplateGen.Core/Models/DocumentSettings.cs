namespace TemplateGen.Core.Models;

using System.Text.Json.Serialization;

public record DocumentSettings(
    [property: JsonPropertyName("page_size")] string PageSize, // e.g., "A4"
    [property: JsonPropertyName("orientation")] string Orientation, // e.g., "portrait"
    [property: JsonPropertyName("margins")] Margins Margins
);

public record Margins(
    [property: JsonPropertyName("top_twips")] int Top,
    [property: JsonPropertyName("bottom_twips")] int Bottom,
    [property: JsonPropertyName("left_twips")] int Left,
    [property: JsonPropertyName("right_twips")] int Right
);
