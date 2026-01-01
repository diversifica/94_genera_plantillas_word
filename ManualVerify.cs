using System;
using System.IO;
using System.Linq;
using TemplateGen.Core.Services;
using TemplateGen.Core.Models;
using DocumentFormat.OpenXml.Packaging;
using System.Collections.Generic;

namespace ManualVerify
{
    class Program
    {
        static void Main(string[] args)
        {
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "manual_verify.docx");
            Console.WriteLine($"Generating to {outputPath}...");

            var numberingConfig = new NumberingConfig(
                new HeadingNumberingConfig("heading-1", new List<NumberingLevelConfig>
                {
                    new NumberingLevelConfig(0, "Decimal", "%1.", 1, new IndentationConfig(720, 360, null))
                }, null),
                new ListNumberingConfig(
                    new List<ListConfig> { new ListConfig("list-1", 0, new IndentationConfig(720, 360, null)) },
                    new List<ListConfig> { new ListConfig("bullet-1", 0, new IndentationConfig(720, 360, null)) }
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
            generator.Generate(profile, outputPath);

            Console.WriteLine("Generation complete.");

            // Verify
            using (var doc = WordprocessingDocument.Open(outputPath, false))
            {
                var numPart = doc.MainDocumentPart.NumberingDefinitionsPart;
                if (numPart == null)
                {
                    Console.WriteLine("ERROR: NumberingDefinitionsPart is null.");
                    return;
                }
                
                Console.WriteLine("NumberingDefinitionsPart found.");
                Console.WriteLine("Inner XML content sample:");
                var xml = numPart.Numbering.InnerXml;
                Console.WriteLine(xml.Substring(0, Math.Min(500, xml.Length)));

                if (xml.Contains("w:numId") && xml.Contains("w:abstractNumId"))
                {
                    Console.WriteLine("SUCCESS: XML contains expected elements.");
                }
                else
                {
                     Console.WriteLine("WARNING: XML might be missing elements.");
                }
            }
        }
    }
}
