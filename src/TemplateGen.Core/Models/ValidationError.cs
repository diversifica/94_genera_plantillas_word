namespace TemplateGen.Core.Models;

public record ValidationError(
    string JsonPath,
    string Message
)
{
    public string Severity { get; init; } = "Error"; // Default to Error
}
