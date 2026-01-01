using System.IO;
using Xunit;
using TemplateGen.Core.Services;
using TemplateGen.Core.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

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
            new ProfileMetadata("test_client", "Test Client", "1.0.0", "Desc"),
            null,
            null
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

    [Fact]
    public void Generate_WithSettings_AppliesPageSetupAndStyles()
    {
        // Arrange
        var outputPath = Path.Combine(Path.GetTempPath(), "test_output_settings.docx");
        var profile = new TemplateProfile(
            "1.0.0", 
            new ProfileMetadata("test", "Test", "1.0", null),
            new DocumentSettings("A4", "landscape", new Margins(1000, 1000, 1000, 1000)),
            new StylesConfig(new System.Collections.Generic.List<ParagraphStyleConfig> 
            {
                new ParagraphStyleConfig("Normal", "Normal", null, new StyleProperties(
                    new RunPropertiesConfig(11, false, false),
                    new ParagraphPropertiesConfig("left", 0, null)
                ))
            })
        );
        var generator = new WordGeneratorService();

        try
        {
            // Act
            generator.Generate(profile, outputPath);

            // Assert
            using (var doc = WordprocessingDocument.Open(outputPath, false))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var sectionProps = body.Elements<SectionProperties>().FirstOrDefault();
                Assert.NotNull(sectionProps);
                
                // Verify Page Size (Landscape A4: Width > Height)
                var pageSize = sectionProps.Elements<PageSize>().FirstOrDefault();
                Assert.NotNull(pageSize);
                Assert.Equal(PageOrientationValues.Landscape, pageSize.Orient.Value);
                Assert.True(pageSize.Width.Value > pageSize.Height.Value);

                // Verify Styles Part
                var stylePart = doc.MainDocumentPart.StyleDefinitionsPart;
                Assert.NotNull(stylePart);
                Assert.NotNull(stylePart.Styles);
            }
        }
        finally
        {
            if (File.Exists(outputPath)) File.Delete(outputPath);
        }
    }
}
