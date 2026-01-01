using System.IO;
using Xunit;
using TemplateGen.Core.Services;
using TemplateGen.Core.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;

namespace TemplateGen.Tests;

public class WordGeneratorTests
{
    [Fact]
    public void Generate_CreatesValidDotxTemplate()
    {
        // Arrange
        var outputPath = Path.Combine(Path.GetTempPath(), "test_template.dotx");
        var profile = new TemplateProfile(
            "1.0.0", 
            new ProfileMetadata("test_client", "Test Client", "1.0.0", "Desc"),
            null,
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

            // Verify it's a valid OpenXML template
            using (var doc = WordprocessingDocument.Open(outputPath, false))
            {
                Assert.Equal(WordprocessingDocumentType.Template, doc.DocumentType);
                Assert.NotNull(doc.MainDocumentPart);
                Assert.NotNull(doc.MainDocumentPart.Document);
                var body = doc.MainDocumentPart.Document.Body;
                Assert.NotNull(body);
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
                    new ParagraphPropertiesConfig("left", 0, null, null)
                ))
            }),
            null
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

    [Fact]
    public void Generate_WithNumbering_CreatesNumberingPart()
    {
        // Arrange
        var outputPath = Path.Combine(Path.GetTempPath(), "test_output_numbering.docx");
        var numberingConfig = new NumberingConfig(
            new HeadingNumberingConfig("heading-1", new System.Collections.Generic.List<NumberingLevelConfig>
            {
                new NumberingLevelConfig(0, "Decimal", "%1.", 1, new IndentationConfig(720, 360, null))
            }, null),
            new ListNumberingConfig(
                new System.Collections.Generic.List<ListConfig> { new ListConfig("list-1", 0, new IndentationConfig(720, 360, null)) },
                new System.Collections.Generic.List<ListConfig> { new ListConfig("bullet-1", 0, new IndentationConfig(720, 360, null)) }
            )
        );

        var profile = new TemplateProfile(
            "1.0.0", 
            new ProfileMetadata("test", "Test", "1.0", null),
            null,
            null,
            numberingConfig
        );
        var generator = new WordGeneratorService();

        try
        {
            // Act
            generator.Generate(profile, outputPath);

            // Assert
            using (var doc = WordprocessingDocument.Open(outputPath, false))
            {
                var numberingPart = doc.MainDocumentPart.NumberingDefinitionsPart;
                Assert.NotNull(numberingPart);
                Assert.NotNull(numberingPart.Numbering);
                
                // Verify we have AbstractNum and Num elements
                // checking elements existence
                var numbering = numberingPart.Numbering;
                Assert.True(numbering.ChildElements.Any(e => e.LocalName == "abstractNum"));
                Assert.True(numbering.ChildElements.Any(e => e.LocalName == "num"));
            }
        }
        finally
        {
            // if (File.Exists(outputPath)) File.Delete(outputPath); 
            // Keep specific test output for debug if needed? No, auto-delete.
            if (File.Exists(outputPath)) File.Delete(outputPath);
        }
    }
}
