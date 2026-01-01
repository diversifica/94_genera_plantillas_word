using System.IO;
using Xunit;
using TemplateGen.Core.Services;
using TemplateGen.Core.Models;
using DocumentFormat.OpenXml.Packaging;

namespace TemplateGen.Tests;

public class WordGeneratorTests
{
    [Fact]
    public void Generate_CreatesValidDocxFile()
    {
        // Arrange
        var outputPath = Path.Combine(Path.GetTempPath(), "test_output.docx");
        var profile = new TemplateProfile(
            "1.0.0", 
            new ProfileMetadata("test_client", "Test Client", "1.0.0", "Desc")
        );
        var generator = new WordGeneratorService();

        try 
        {
            // Act
            generator.Generate(profile, outputPath);

            // Assert
            Assert.True(File.Exists(outputPath));

            // Verify it's a valid OpenXML document
            using (var doc = WordprocessingDocument.Open(outputPath, false))
            {
                Assert.NotNull(doc.MainDocumentPart);
                Assert.NotNull(doc.MainDocumentPart.Document);
                var body = doc.MainDocumentPart.Document.Body;
                Assert.NotNull(body);
                // Check if our text is there
                Assert.Contains("Test Client", body.InnerText);
            }
        }
        finally
        {
            if (File.Exists(outputPath)) File.Delete(outputPath);
        }
    }
}
