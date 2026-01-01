namespace TemplateGen.Core.Models;

public record ValidationError(
    string Severity,
    string JsonPath,
    string Message
);
