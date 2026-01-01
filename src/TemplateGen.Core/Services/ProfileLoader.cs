namespace TemplateGen.Core.Services;

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TemplateGen.Core.Models;

public class ProfileLoader
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public async Task<TemplateProfile> LoadAsync(string path)
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
        try
        {
            var profile = await JsonSerializer.DeserializeAsync<TemplateProfile>(stream, _jsonOptions);
            return profile ?? throw new InvalidOperationException("Failed to deserialize profile (result was null).");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Error parsing profile JSON: {ex.Message}", ex);
        }
    }
}
