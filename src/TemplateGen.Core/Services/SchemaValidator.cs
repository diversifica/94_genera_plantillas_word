namespace TemplateGen.Core.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using TemplateGen.Core.Models;

public class SchemaValidator
{
    private readonly JsonSchema _schema;

    public SchemaValidator(string schemaPath)
    {
        if (!File.Exists(schemaPath))
        {
            throw new FileNotFoundException($"Schema file not found at: {schemaPath}", schemaPath);
        }

        try
        {
            var schemaText = File.ReadAllText(schemaPath);
            _schema = JsonSchema.FromText(schemaText);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to parse JSON Schema: {ex.Message}", ex);
        }
    }

    public IEnumerable<ValidationError> Validate(JsonNode document)
    {
        // Workaround for ambiguous Evaluate overload or missing JsonNode support in this build context
        var element = JsonSerializer.SerializeToElement(document);
        var result = _schema.Evaluate(element, new EvaluationOptions
        {
            OutputFormat = OutputFormat.List
        });

        if (result.IsValid)
        {
            return Enumerable.Empty<ValidationError>();
        }

        if (result.Details == null)
        {
             return new[] { new ValidationError("error", "$", "Unknown validation error") };
        }

        // Map JsonSchema.Net errors to our ValidationError DTO
        // OutputFormat.List produces a flat list of errors in Details or sub-trees.
        // We filter for nodes that have actual errors (IsValid == false) and have a message.
        
        // Note: The structure of Details depends heavily on OutputFormat.
        // For List format, we usually look at the nested items that failed.
        
        var errors = new List<ValidationError>();

        void CollectErrors(IEnumerable<EvaluationResults> results)
        {
            foreach (var item in results)
            {
                if (!item.IsValid && item.Errors != null)
                {
                     foreach (var error in item.Errors)
                     {
                         // Location can be InstanceLocation or SchemaLocation. Instance is what we want for user reporting.
                         var path = item.InstanceLocation.ToString();
                         // The dictionary key is the keyword that failed (e.g. "type", "required")
                         // The value is the error message.
                         errors.Add(new ValidationError("error", path, $"{error.Key}: {error.Value}"));
                     }
                }
                
                // Recurse if needed, though OutputFormat.List usually flattens meaningful errors into the top level list or specific children
                if (item.Details != null)
                {
                    CollectErrors(item.Details);
                }
            }
        }
        
        // Usually result.Details contains the list in List mode
        CollectErrors(new[] { result });

        return errors;
    }
}
