namespace TemplateGen.Core.Models;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public record DocumentContent(
    [property: JsonPropertyName("document_id")] string DocumentId,
    [property: JsonPropertyName("language")] string Language,
    [property: JsonPropertyName("sections")] List<SectionContent> Sections
);

public record SectionContent(
    [property: JsonPropertyName("section_id")] string SectionId,
    [property: JsonPropertyName("content")] List<ContentElement> Content
);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ParagraphElement), typeDiscriminator: "paragraph")]
[JsonDerivedType(typeof(HeadingElement), typeDiscriminator: "heading")]
[JsonDerivedType(typeof(ListElement), typeDiscriminator: "list")]
[JsonDerivedType(typeof(TableElement), typeDiscriminator: "table")]
[JsonDerivedType(typeof(ImageElement), typeDiscriminator: "image")]
public record ContentElement();

public record ParagraphElement(
    [property: JsonPropertyName("style_id")] string StyleId,
    [property: JsonPropertyName("text")] string Text
) : ContentElement;

public record HeadingElement(
    [property: JsonPropertyName("level")] int Level,
    [property: JsonPropertyName("text")] string Text
) : ContentElement;

public record ListElement(
    [property: JsonPropertyName("list_type")] string ListType, // "numbered" or "bullet"
    [property: JsonPropertyName("items")] List<ListItemElement> Items
) : ContentElement;

public record ListItemElement(
    [property: JsonPropertyName("text")] string Text
);

public record TableElement(
    [property: JsonPropertyName("rows")] List<TableRowElement> Rows
) : ContentElement;

public record TableRowElement(
    [property: JsonPropertyName("cells")] List<TableCellElement> Cells
);

public record TableCellElement(
    [property: JsonPropertyName("content")] List<ContentElement> Content
);

public record ImageElement(
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("width")] long? Width,
    [property: JsonPropertyName("height")] long? Height,
    [property: JsonPropertyName("alt_text")] string? AltText
) : ContentElement;
