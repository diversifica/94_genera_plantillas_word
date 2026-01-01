namespace TemplateGen.Core.Services;

using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TemplateGen.Core.Models;

public class ContentGeneratorService
{
    private const string W_NAMESPACE = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

    // Hardcoded assumptions for MVP based on NumberingDefinitionsGenerator deterministic order
    private const int HEADING_NUM_ID = 1;
    private const int NUMBERED_LIST_NUM_ID = 2;
    private const int BULLET_LIST_NUM_ID = 3;

    public void GenerateContent(MainDocumentPart mainPart, DocumentContent content)
    {
        var body = mainPart.Document.Body;
        if (body == null) return;

        foreach (var section in content.Sections)
        {
            foreach (var element in section.Content)
            {
                if (element is ParagraphElement pElem)
                {
                    body.Append(CreateParagraph(pElem));
                }
                else if (element is HeadingElement hElem)
                {
                    body.Append(CreateHeading(hElem));
                }
                else if (element is ListElement lElem)
                {
                    var items = CreateListItems(lElem);
                    foreach(var item in items)
                    {
                        body.Append(item);
                    }
                }
            }
        }
    }

    private Paragraph CreateParagraph(ParagraphElement element)
    {
        var p = new Paragraph();
        
        var pPr = new ParagraphProperties();
        pPr.ParagraphStyleId = new ParagraphStyleId() { Val = element.StyleId };
        p.Append(pPr);

        var run = new Run();
        run.Append(new Text(element.Text));
        p.Append(run);

        return p;
    }

    private Paragraph CreateHeading(HeadingElement element)
    {
        var p = new Paragraph();
        
        var pPr = new ParagraphProperties();
        // Map Heading Element Level 1 -> "Heading1", Level 2 -> "Heading2"
        string styleId = $"Heading{element.Level}"; 
        pPr.ParagraphStyleId = new ParagraphStyleId() { Val = styleId };
        
        p.Append(pPr);

        var run = new Run();
        run.Append(new Text(element.Text));
        p.Append(run);

        return p;
    }

    private List<Paragraph> CreateListItems(ListElement element)
    {
        var paragraphs = new List<Paragraph>();
        int numId = element.ListType == "numbered" ? NUMBERED_LIST_NUM_ID : BULLET_LIST_NUM_ID;

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
            run.Append(new Text(item.Text));
            p.Append(run);
            
            paragraphs.Add(p);
        }
        return paragraphs;
    }
}
