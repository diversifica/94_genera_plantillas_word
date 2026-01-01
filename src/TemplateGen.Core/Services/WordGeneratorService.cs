namespace TemplateGen.Core.Services;

using System;
using System.IO;
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
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            // Add simple text for PoC verification
            var paragraph = body.AppendChild(new Paragraph());
            var run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text($"TemplateGen Generated Document for client: {profile.Metadata.ClientName}"));
            
            mainPart.Document.Save();
        }
    }
}
