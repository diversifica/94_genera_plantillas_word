namespace TemplateGen.Core.Services;

using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TemplateGen.Core.Models;

public class WordGeneratorService
{
    public void Generate(TemplateProfile profile, string outputPath)
    {
        if (profile == null) throw new ArgumentNullException(nameof(profile));
        if (string.IsNullOrWhiteSpace(outputPath)) throw new ArgumentException("Output path cannot be empty", nameof(outputPath));

        // Ensure directory exists
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (var document = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
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

            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            // Add simple text for PoC verification
            var paragraph = body.AppendChild(new Paragraph());
            
            // Apply Normal style explicitly to test
            if (profile.Styles?.ParagraphStyles?.Any(s => s.StyleId == "Normal") == true)
            {
               paragraph.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId() { Val = "Normal" });
            }

            var run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text($"TemplateGen Generated Document for client: {profile.Metadata.ClientName}"));
            
            // 2. Apply Page Setup (SectionProperties) matches 'document' settings
            if (profile.Document != null)
            {
                 var sectionProps = new SectionProperties();

                 // Page Size & Orientation
                 if (!string.IsNullOrEmpty(profile.Document.PageSize))
                 {
                     // Approximate A4 size in twips (11906W x 16838H) for Portrait
                     // For PoC we assume A4 if string matches, else default
                     var pageSize = new PageSize() { Width = 11906, Height = 16838 };
                     if (profile.Document.Orientation?.ToLower() == "landscape")
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
                         Left = (uint)Math.Max(0, profile.Document.Margins.Left),
                         Right = (uint)Math.Max(0, profile.Document.Margins.Right),
                         Header = 708, // defaults
                         Footer = 708,
                         Gutter = 0
                     };
                     sectionProps.Append(margins);
                 }

                 body.Append(sectionProps);
            }

            mainPart.Document.Save();
        }
    }
}
