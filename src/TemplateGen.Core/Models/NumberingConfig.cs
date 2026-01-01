namespace TemplateGen.Core.Models;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public record NumberingConfig(
    [property: JsonPropertyName("heading_numbering")] HeadingNumberingConfig? HeadingNumbering,
    [property: JsonPropertyName("list_numbering")] ListNumberingConfig? ListNumbering
);

public record HeadingNumberingConfig(
    [property: JsonPropertyName("scheme_id")] string SchemeId,
    [property: JsonPropertyName("levels")] List<NumberingLevelConfig> Levels,
    [property: JsonPropertyName("style_bindings")] List<StyleBindingConfig>? StyleBindings
);

public record ListNumberingConfig(
     [property: JsonPropertyName("numbered_lists")] List<ListConfig>? NumberedLists,
     [property: JsonPropertyName("bullet_lists")] List<ListConfig>? BulletLists
);

public record ListConfig(
    [property: JsonPropertyName("scheme_id")] string SchemeId,
    [property: JsonPropertyName("level_index")] int LevelIndex,
    [property: JsonPropertyName("indentation")] IndentationConfig? Indentation
);

public record NumberingLevelConfig(
    [property: JsonPropertyName("level_index")] int LevelIndex,
    [property: JsonPropertyName("number_format")] string NumberFormat,
    [property: JsonPropertyName("level_text_pattern")] string LevelTextPattern,
    [property: JsonPropertyName("start_at")] int StartAt,
    [property: JsonPropertyName("indentation")] IndentationConfig? Indentation
);

public record StyleBindingConfig(
    [property: JsonPropertyName("level_index")] int LevelIndex,
    [property: JsonPropertyName("style_id")] string StyleId
);

public record IndentationConfig(
    [property: JsonPropertyName("left_twips")] int LeftTwips,
    [property: JsonPropertyName("hanging_twips")] int? HangingTwips,
    [property: JsonPropertyName("first_line_twips")] int? FirstLineTwips
);
