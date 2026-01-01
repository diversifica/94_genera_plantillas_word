namespace TemplateGen.Core.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using TemplateGen.Core.Models;

public class ContentGeneratorService
{
    private const int NUMBERED_LIST_NUM_ID = 2;
    private const int BULLET_LIST_NUM_ID = 3;
    private readonly ILogger<ContentGeneratorService> _logger;

    public ContentGeneratorService(ILogger<ContentGeneratorService>? logger = null)
    {
        _logger = logger ?? NullLogger<ContentGeneratorService>.Instance;
    }

    public void GenerateContent(MainDocumentPart mainPart, DocumentContent content)
    {
        if (mainPart.Document == null) mainPart.Document = new Document();
        if (mainPart.Document.Body == null) mainPart.Document.Body = new Body();
        
        var body = mainPart.Document.Body;

        if (content.Sections == null) return;

        foreach (var section in content.Sections)
        {
            if (section.Content == null) continue;
            foreach (var element in section.Content)
            {
                var generatedElements = GenerateElement(mainPart, element);
                foreach (var generatedElement in generatedElements)
                {
                    body.Append(generatedElement);
                }
            }
        }
    }

    private IEnumerable<OpenXmlElement> GenerateElement(MainDocumentPart mainPart, ContentElement element)
    {
        if (element is ParagraphElement pElem)
        {
            yield return CreateParagraph(pElem);
        }
        else if (element is HeadingElement hElem)
        {
            yield return CreateHeading(hElem);
        }
        else if (element is ListElement lElem)
        {
            foreach (var p in CreateListItems(lElem))
            {
                yield return p;
            }
        }
        else if (element is TableElement tElem)
        {
            yield return CreateTable(mainPart, tElem);
        }
        else if (element is ImageElement iElem)
        {
            yield return CreateImage(mainPart, iElem);
        }
    }

    private Paragraph CreateParagraph(ParagraphElement element)
    {
        var p = new Paragraph();
        var pPr = new ParagraphProperties();
        if (!string.IsNullOrEmpty(element.StyleId))
        {
            pPr.ParagraphStyleId = new ParagraphStyleId() { Val = element.StyleId };
        }
        p.Append(pPr);
        var run = new Run();
        run.Append(new Text(element.Text ?? string.Empty));
        p.Append(run);
        return p;
    }

    private Paragraph CreateHeading(HeadingElement element)
    {
        var p = new Paragraph();
        var pPr = new ParagraphProperties();
        pPr.ParagraphStyleId = new ParagraphStyleId() { Val = $"Heading{element.Level}" };
        p.Append(pPr);
        var run = new Run();
        run.Append(new Text(element.Text ?? string.Empty));
        p.Append(run);
        return p;
    }

    private List<Paragraph> CreateListItems(ListElement element)
    {
        var paragraphs = new List<Paragraph>();
        int numId = element.ListType == "numbered" ? NUMBERED_LIST_NUM_ID : BULLET_LIST_NUM_ID;

        if (element.Items != null)
        {
            foreach (var item in element.Items)
            {
                var p = new Paragraph();
                var pPr = new ParagraphProperties();
                pPr.ParagraphStyleId = new ParagraphStyleId() { Val = "ListParagraph" };
                var numPr = new NumberingProperties();
                numPr.Append(new NumberingLevelReference() { Val = 0 });
                numPr.Append(new NumberingId() { Val = numId });
                pPr.Append(numPr);
                p.Append(pPr);
                var run = new Run();
                run.Append(new Text(item.Text ?? string.Empty));
                p.Append(run);
                paragraphs.Add(p);
            }
        }
        return paragraphs;
    }

    private Table CreateTable(MainDocumentPart mainPart, TableElement tblElem)
    {
        var table = new Table();
        
        var tblPr = new TableProperties(
            new TableBorders(
                new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
            ),
            new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct } 
        );
        table.Append(tblPr);

        if (tblElem.Rows != null)
        {
            foreach (var rElem in tblElem.Rows)
            {
                var tr = new TableRow();
                if (rElem.Cells != null)
                {
                    foreach (var cElem in rElem.Cells)
                    {
                        var tc = new TableCell();
                        if (cElem.Content != null)
                        {
                            foreach (var contentItem in cElem.Content)
                            {
                                var cellContents = GenerateElement(mainPart, contentItem);
                                foreach (var cellContent in cellContents)
                                {
                                    tc.Append(cellContent);
                                }
                            }
                        }
                        
                        if (!tc.Descendants<Paragraph>().Any())
                        {
                            tc.Append(new Paragraph());
                        }

                        tr.Append(tc);
                    }
                }
                table.Append(tr);
            }
        }

        return table;
    }

    private Paragraph CreateImage(MainDocumentPart mainPart, ImageElement imgElem)
    {
        if (string.IsNullOrEmpty(imgElem.Source) || !File.Exists(imgElem.Source))
        {
            _logger.LogWarning("Image file not found: {ImagePath}. Inserting placeholder text.", imgElem.Source);
            var pPlaceholder = new Paragraph();
            pPlaceholder.Append(new Run(new Text($"[MISSING IMAGE: {imgElem.Source}]")));
            return pPlaceholder;
        }

        var partType = ImagePartType.Jpeg;
        var ext = Path.GetExtension(imgElem.Source);
        if (!string.IsNullOrEmpty(ext) && ext.Equals(".png", StringComparison.OrdinalIgnoreCase))
        {
             partType = ImagePartType.Png;
        }

        var imgPart = mainPart.AddImagePart(partType);
        using (var fs = new FileStream(imgElem.Source, FileMode.Open, FileAccess.Read))
        {
            imgPart.FeedData(fs);
        }
        var relId = mainPart.GetIdOfPart(imgPart);

        long cx = (imgElem.Width ?? 100) * 9525;
        long cy = (imgElem.Height ?? 100) * 9525;
        var fileName = Path.GetFileName(imgElem.Source) ?? "image";
        var alt = imgElem.AltText ?? "Image";

        var drawingFrame = new Drawing(
             new DW.Inline(
                 new DW.Extent() { Cx = cx, Cy = cy },
                 new DW.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
                 new DW.DocProperties() { Id = (UInt32Value)1U, Name = fileName, Description = alt },
                 new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoChangeAspect = true }),
                 new A.Graphic(
                     new A.GraphicData(
                         new PIC.Picture(
                             new PIC.NonVisualPictureProperties(
                                 new PIC.NonVisualDrawingProperties() { Id = (UInt32Value)0U, Name = fileName },
                                 new PIC.NonVisualPictureDrawingProperties()),
                             new PIC.BlipFill(
                                 new A.Blip() { Embed = relId, CompressionState = A.BlipCompressionValues.Print },
                                 new A.Stretch(new A.FillRectangle())),
                             new PIC.ShapeProperties(
                                 new A.Transform2D(
                                     new A.Offset() { X = 0L, Y = 0L },
                                     new A.Extents() { Cx = cx, Cy = cy }),
                                 new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }))
                     ) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
             ) { DistanceFromTop = (UInt32Value)0U, DistanceFromBottom = (UInt32Value)0U, DistanceFromLeft = (UInt32Value)0U, DistanceFromRight = (UInt32Value)0U, EditId = "50D07946" }
         );

        var para = new Paragraph(new Run(drawingFrame));
        return para;
    }
}
