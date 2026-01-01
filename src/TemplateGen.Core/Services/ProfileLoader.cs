namespace TemplateGen.Core.Services;

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using TemplateGen.Core.Models;


public class ProfileLoader
{
    private const string SUPPORTED_SCHEMA_VERSION = "1.0.0";
    
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private readonly SchemaValidator? _validator;

    public ProfileLoader(SchemaValidator? validator = null)
    {
        _validator = validator;
    }

    public async Task<(TemplateProfile Profile, IEnumerable<ValidationError> ValidationErrors)> LoadWithValidationAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Profile path cannot be empty.", nameof(path));
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Profile file not found at: {path}", path);
        }

        using var stream = File.OpenRead(path);
        // We need to read the stream for validation (as JsonNode) AND for deserialization.
        // Option 1: Read to string/byte array first.
        
        JsonNode? jsonNode;
        try 
        {
             jsonNode = await JsonNode.ParseAsync(stream);
        }
        catch (JsonException ex)
        {
            // If it's invalid JSON, we can't even validate schema.
            throw new InvalidOperationException($"Error parsing profile JSON: {ex.Message}", ex);
        }

        if (jsonNode == null)
             throw new InvalidOperationException("Profile JSON was null or empty.");

        var validationErrors = new List<ValidationError>();
        if (_validator != null)
        {
            validationErrors.AddRange(_validator.Validate(jsonNode));
        }

        // If strict, we might want to stop here. But usually we return errors and let the caller decide.
        // However, if the JSON is valid structure-wise, we can attempt deserialize.
        
        TemplateProfile? profile = null;
        try 
        {
            profile = jsonNode.Deserialize<TemplateProfile>(_jsonOptions);
        }
         catch (JsonException ex)
        {
             throw new InvalidOperationException($"Error deserializing profile: {ex.Message}", ex);
        }
        
        if (profile == null)
             throw new InvalidOperationException("Failed to deserialize profile (result was null).");

        // Validate schema version
        ValidateSchemaVersion(profile);

        // Perform semantic validation after deserialization
        var semanticValidator = new SemanticValidator();
        validationErrors.AddRange(semanticValidator.Validate(profile));

        return (profile, validationErrors);
    }
    
    // Kept for backward compatibility or simple usage, but redirects to new logic
    public async Task<TemplateProfile> LoadAsync(string path)
    {
        var (profile, errors) = await LoadWithValidationAsync(path);
        if (errors.Any())
        {
            // For simple LoadAsync, we might want to throw if there are generic errors, 
            // but effectively we are deprecating this in favor of the validation-aware one for the CLI.
            // Or we just ignore errors here (not recommended).
            // Let's assume this method assumes "no validator" or "ignore errors" unless configured.
        }
        return profile;
    }

    private void ValidateSchemaVersion(TemplateProfile profile)
    {
        if (string.IsNullOrEmpty(profile.SchemaVersion))
        {
            throw new InvalidOperationException(
                "Profile schema version is missing. All profiles must specify a schema_version.");
        }

        if (profile.SchemaVersion != SUPPORTED_SCHEMA_VERSION)
        {
            throw new InvalidOperationException(
                $"Unsupported schema version: {profile.SchemaVersion}. " +
                $"This version of TemplateGen supports schema version {SUPPORTED_SCHEMA_VERSION}. " +
                $"Please update your profile or use a compatible version of TemplateGen.");
        }
    }
}
