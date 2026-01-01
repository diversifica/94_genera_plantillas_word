using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Xunit;
using TemplateGen.Core.Services;

namespace TemplateGen.Tests;

public class SchemaValidatorTests
{
    private const string SchemaContent = @"
    {
      ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
      ""type"": ""object"",
      ""properties"": {
        ""name"": { ""type"": ""string"" },
        ""age"": { ""type"": ""integer"" }
      },
      ""required"": [""name""]
    }";

    [Fact]
    public void Validate_ValidJson_ReturnsEmptyErrors()
    {
        var schemaPath = Path.GetTempFileName();
        File.WriteAllText(schemaPath, SchemaContent);

        try
        {
            var validator = new SchemaValidator(schemaPath);
            var json = JsonNode.Parse("{\"name\": \"Alice\", \"age\": 30}");

            var errors = validator.Validate(json!);
            Assert.Empty(errors);
        }
        finally
        {
            File.Delete(schemaPath);
        }
    }

    [Fact]
    public void Validate_MissingRequiredField_ReturnsError()
    {
        var schemaPath = Path.GetTempFileName();
        File.WriteAllText(schemaPath, SchemaContent);

        try
        {
            var validator = new SchemaValidator(schemaPath);
            var json = JsonNode.Parse("{\"age\": 30}");

            var errors = validator.Validate(json!).ToList();
            Assert.NotEmpty(errors);
            Assert.Contains(errors, e => e.Message.Contains("name")); // 'required' error usually mentions the missing property
        }
        finally
        {
            File.Delete(schemaPath);
        }
    }

    [Fact]
    public void Validate_WrongType_ReturnsError()
    {
        var schemaPath = Path.GetTempFileName();
        File.WriteAllText(schemaPath, SchemaContent);

        try
        {
            var validator = new SchemaValidator(schemaPath);
            var json = JsonNode.Parse("{\"name\": \"Alice\", \"age\": \"thirty\"}");

            var errors = validator.Validate(json!).ToList();
            Assert.NotEmpty(errors);
             // 'type' error
        }
        finally
        {
            File.Delete(schemaPath);
        }
    }
}
