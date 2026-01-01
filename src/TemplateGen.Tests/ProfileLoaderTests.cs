using System.IO;
using System.Threading.Tasks;
using Xunit;
using TemplateGen.Core.Services;

namespace TemplateGen.Tests;

public class ProfileLoaderTests
{
    [Fact]
    public async Task LoadAsync_WithValidPath_ReturnsProfile()
    {
        // Arrange
        // We assume the test runs from bin/Debug/..., so we need to locate the profile relative to repo root
        // Or we can create a temporary file.
        // For reliability, I'll create a temp file.
        var jsonContent = @"
        {
            ""schema_version"": ""1.0.0"",
            ""metadata"": {
                ""client_id"": ""test_client"",
                ""client_name"": ""Test Client"",
                ""profile_version"": ""1.0.0"",
                ""description"": ""Test Description""
            }
        }";
        var tempFile = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFile, jsonContent);

        try
        {
            var loader = new ProfileLoader();

            // Act
            var profile = await loader.LoadAsync(tempFile);

            // Assert
            Assert.NotNull(profile);
            Assert.Equal("1.0.0", profile.SchemaVersion);
            Assert.Equal("test_client", profile.Metadata.ClientId);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task LoadAsync_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        var loader = new ProfileLoader();
        await Assert.ThrowsAsync<FileNotFoundException>(() => loader.LoadAsync("non_existent.json"));
    }
}
