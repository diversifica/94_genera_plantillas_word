namespace TemplateGen.Core.Services;

using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TemplateGen.Core.Models;

public class WordGeneratorService
{
    private readonly ILogger<WordGeneratorService> _logger;

    public WordGeneratorService(ILogger<WordGeneratorService>? logger = null)
    {
        _logger = logger ?? NullLogger<WordGeneratorService>.Instance;
    }

    public void Generate(TemplateProfile profile, string outputPath, DocumentContent? content = null, bool includeToc = false)
    {
        _logger.LogInformation("Starting template generation for output: {OutputPath}", outputPath);
        if (profile == null) throw new ArgumentNullException(nameof(profile));
        if (string.IsNullOrWhiteSpace(outputPath)) throw new ArgumentException("Output path cannot be empty", nameof(outputPath));

        // Ensure directory exists
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Determine document type based on extension
        var documentType = Path.GetExtension(outputPath).Equals(".docx", StringComparison.OrdinalIgnoreCase) 
            ? WordprocessingDocumentType.Document 
            : WordprocessingDocumentType.Template;

        using (var document = WordprocessingDocument.Create(outputPath, documentType))
        {
            // Create MainDocumentPart
            var mainPart = document.AddMainDocumentPart();
            
            // 1. Generate Styles if config exists
            if (profile.Styles != null)
            {
                var stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
                var styleGenerator = new StyleDefinitionsGenerator();
                stylePart.Styles = styleGenerator.GenerateStyles(profile.Styles);
                stylePart.Styles.Save();
            }

            // 2. Generate Numbering if config exists
            if (profile.Numbering != null)
            {
                var numberingPart = mainPart.AddNewPart<NumberingDefinitionsPart>();
                var numberingGenerator = new NumberingDefinitionsGenerator();
                numberingPart.Numbering = numberingGenerator.GenerateNumbering(profile.Numbering);
                numberingPart.Numbering.Save();
            }

            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            // 3. Generate Content
            if (content != null)
            {
                var contentGenerator = new ContentGeneratorService(_logger as ILogger<ContentGeneratorService>);
                contentGenerator.GenerateContent(mainPart, content);
            }
            else
            {
                // Default placeholder
                var p = new Paragraph();
                var pPr = new ParagraphProperties();
                pPr.ParagraphStyleId = new ParagraphStyleId() { Val = "Normal" }; 
                p.Append(pPr);
                var r = new Run();
                var t = new Text($"TemplateGen Generated Document for client: {profile.Metadata?.ClientName ?? "Unknown"}");
                r.Append(t);
                p.Append(r);
                body.Append(p);
            }

             // 4. Page Setup & Document Settings (SectionProperties) matches 'document' settings
             // Note: SectionProperties usually goes at the END of the body (for the last section)
            if (profile.Document != null)
            {
                 var sectionProps = new SectionProperties();

                 // Page Size & Orientation
                 if (!string.IsNullOrEmpty(profile.Document.PageSize))
                 {
                     // Approximate A4 size in twips (11906W x 16838H) for Portrait
                     var pageSize = new PageSize() { Width = 11906, Height = 16838 };
                     
                     if ("landscape".Equals(profile.Document.Orientation, StringComparison.OrdinalIgnoreCase))
                     {
                         pageSize.Orient = PageOrientationValues.Landscape;
                         // Swap width/height
                         (pageSize.Width, pageSize.Height) = (pageSize.Height, pageSize.Width);
                     }
                     sectionProps.Append(pageSize);
                 }

                 // Margins
                 if (profile.Document.Margins != null)
                 {
                     var margins = new PageMargin()
                     {
                         Top = profile.Document.Margins.Top,
                         Bottom = profile.Document.Margins.Bottom,
                         Left = (UInt32Value)(uint)profile.Document.Margins.Left,
                         Right = (UInt32Value)(uint)profile.Document.Margins.Right,
                         Header = (UInt32Value)708U, // defaults
                         Footer = (UInt32Value)708U,
                         Gutter = (UInt32Value)0U
                     };
                     sectionProps.Append(margins);
                 }

                 body.Append(sectionProps);
            }

            mainPart.Document.Save();
        }
    }
}
