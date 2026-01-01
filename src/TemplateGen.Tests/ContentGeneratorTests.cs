using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using TemplateGen.Core.Models;
using TemplateGen.Core.Services;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

namespace TemplateGen.Tests
{
    public class ContentGeneratorTests : IDisposable
    {
        private readonly string _testFile;

        public ContentGeneratorTests()
        {
            _testFile = Path.Combine(Path.GetTempPath(), $"test_content_{Guid.NewGuid()}.docx");
        }

        public void Dispose()
        {
            if (File.Exists(_testFile)) File.Delete(_testFile);
        }

        [Fact]
        public void GenerateContent_AddsParagraphsAndHeadings()
        {
            // Arrange
            var content = new DocumentContent("doc1", "en", new List<SectionContent>
            {
                new SectionContent("s1", new List<ContentElement>
                {
                    new ParagraphElement("Normal", "Hello World"),
                    new HeadingElement(1, "Title 1")
                })
            });

            using (var doc = WordprocessingDocument.Create(_testFile, WordprocessingDocumentType.Document))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());

                var service = new ContentGeneratorService();

                // Act
                service.GenerateContent(mainPart, content);
                mainPart.Document.Save();
            }

            // Assert
            using (var doc = WordprocessingDocument.Open(_testFile, false))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var pars = body.Elements<Paragraph>().ToList();
                
                Assert.Equal(2, pars.Count);
                
                // P1: Normal
                Assert.Equal("Hello World", pars[0].InnerText);
                Assert.Equal("Normal", pars[0].ParagraphProperties.ParagraphStyleId.Val.Value);

                // P2: Heading 1
                Assert.Equal("Title 1", pars[1].InnerText);
                Assert.Equal("Heading1", pars[1].ParagraphProperties.ParagraphStyleId.Val.Value);
            }
        }

        [Fact]
        public void GenerateContent_AddsListItems()
        {
            // Arrange
            var content = new DocumentContent("doc1", "en", new List<SectionContent>
            {
                new SectionContent("s1", new List<ContentElement>
                {
                    new ListElement("numbered", new List<ListItemElement>
                    {
                        new ListItemElement("Item 1"),
                        new ListItemElement("Item 2")
                    })
                })
            });

            using (var doc = WordprocessingDocument.Create(_testFile, WordprocessingDocumentType.Document))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());
                
                var service = new ContentGeneratorService();

                // Act
                service.GenerateContent(mainPart, content);
                mainPart.Document.Save();
            }

            // Assert
            using (var doc = WordprocessingDocument.Open(_testFile, false))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var pars = body.Elements<Paragraph>().ToList();
                
                Assert.Equal(2, pars.Count);
                
                foreach(var p in pars)
                {
                    Assert.NotNull(p.ParagraphProperties.NumberingProperties);
                    Assert.Equal(2, p.ParagraphProperties.NumberingProperties.NumberingId.Val.Value); // Numbered default ID
                }
            }
        }
        [Fact]
        public void GenerateContent_AddsTable()
        {
            // Arrange
            var content = new DocumentContent("doc1", "en", new List<SectionContent>
            {
                new SectionContent("s1", new List<ContentElement>
                {
                    new TableElement(new List<TableRowElement>
                    {
                        new TableRowElement(new List<TableCellElement>
                        {
                            new TableCellElement(new List<ContentElement>
                            {
                                new ParagraphElement("Normal", "Cell Text")
                            })
                        })
                    })
                })
            });

            using (var doc = WordprocessingDocument.Create(_testFile, WordprocessingDocumentType.Document))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());
                
                var service = new ContentGeneratorService();

                // Act
                service.GenerateContent(mainPart, content);
                mainPart.Document.Save();
            }

            // Assert
            using (var doc = WordprocessingDocument.Open(_testFile, false))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var table = body.Elements<Table>().FirstOrDefault();
                
                Assert.NotNull(table);
                var row = table.Elements<TableRow>().FirstOrDefault();
                Assert.NotNull(row);
                var cell = row.Elements<TableCell>().FirstOrDefault();
                Assert.NotNull(cell);
                var p = cell.Elements<Paragraph>().FirstOrDefault();
                Assert.NotNull(p);
                Assert.Equal("Cell Text", p.InnerText);
            }
        }

        [Fact]
        public void GenerateContent_AddsMissingImagePlaceholder()
        {
            // Arrange
            var content = new DocumentContent("doc1", "en", new List<SectionContent>
            {
                new SectionContent("s1", new List<ContentElement>
                {
                    new ImageElement("non_existent_image.png", 100, 100, "Alt")
                })
            });

            using (var doc = WordprocessingDocument.Create(_testFile, WordprocessingDocumentType.Document))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());
                
                var service = new ContentGeneratorService();

                // Act
                service.GenerateContent(mainPart, content);
                mainPart.Document.Save();
            }

            // Assert
            using (var doc = WordprocessingDocument.Open(_testFile, false))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var p = body.Elements<Paragraph>().FirstOrDefault();
                Assert.NotNull(p);
                Assert.Contains("[MISSING IMAGE", p.InnerText);
            }
        }
    }
}
