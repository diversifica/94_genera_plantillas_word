namespace TemplateGen.Core.Models;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public record StylesConfig(
    [property: JsonPropertyName("paragraph_styles")] List<ParagraphStyleConfig> ParagraphStyles
);

public record ParagraphStyleConfig(
    [property: JsonPropertyName("style_id")] string StyleId,
    [property: JsonPropertyName("style_name")] string StyleName,
    [property: JsonPropertyName("based_on")] string? BasedOn,
    [property: JsonPropertyName("properties")] StyleProperties Properties
);

public record StyleProperties(
    [property: JsonPropertyName("run")] RunPropertiesConfig? Run,
    [property: JsonPropertyName("paragraph")] ParagraphPropertiesConfig? Paragraph
);

public record RunPropertiesConfig(
    [property: JsonPropertyName("font_size_points")] int? FontSizeMembers,
    [property: JsonPropertyName("bold")] bool? Bold,
    [property: JsonPropertyName("italic")] bool? Italic,
    [property: JsonPropertyName("underline")] string? Underline
);

public record ParagraphPropertiesConfig(
    [property: JsonPropertyName("alignment")] string? Alignment,
    [property: JsonPropertyName("spacing_after_twips")] int? SpacingAfter,
    [property: JsonPropertyName("line_spacing")] LineSpacingConfig? LineSpacing,
    [property: JsonPropertyName("indentation")] IndentationConfig? Indentation
);

public record LineSpacingConfig(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("value")] double Value
);
