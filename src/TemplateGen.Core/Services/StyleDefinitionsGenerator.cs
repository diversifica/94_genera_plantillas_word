namespace TemplateGen.Core.Services;

using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;
using TemplateGen.Core.Models;

public class StyleDefinitionsGenerator
{
    public Styles GenerateStyles(StylesConfig config)
    {
        var styles = new Styles();

        // Add default DocDefaults if needed (basic OpenXML requirement often)
        var docDefaults = new DocDefaults();
        
        var rPrDefault = new RunPropertiesDefault(new RunPropertiesBaseStyle());
        var pPrDefault = new ParagraphPropertiesDefault(
            new ParagraphPropertiesBaseStyle(
                new SpacingBetweenLines() { After = "160", Line = "259", LineRule = LineSpacingRuleValues.Auto }
            )
        );

        docDefaults.Append(rPrDefault);
        docDefaults.Append(pPrDefault);
        
        styles.Append(docDefaults); 

        if (config.ParagraphStyles != null)
        {
            foreach (var styleConfig in config.ParagraphStyles)
            {
                var style = CreateParagraphStyle(styleConfig);
                styles.Append(style);
            }
        }

        return styles;
    }

    private Style CreateParagraphStyle(ParagraphStyleConfig config)
    {
        var style = new Style()
        {
            Type = StyleValues.Paragraph,
            StyleId = config.StyleId,
            CustomStyle = true
        };

        style.Append(new StyleName() { Val = config.StyleName });
        
        if (!string.IsNullOrEmpty(config.BasedOn))
        {
            style.Append(new BasedOn() { Val = config.BasedOn });
        }

        // Style Properties
        var pPr = new StyleParagraphProperties();
        
        if (config.Properties?.Paragraph != null)
        {
            var pConfig = config.Properties.Paragraph;
            
            // Alignment
            if (!string.IsNullOrEmpty(pConfig.Alignment))
            {
                var justification = new Justification();
                switch (pConfig.Alignment.ToLower())
                {
                    case "center": justification.Val = JustificationValues.Center; break;
                    case "right": justification.Val = JustificationValues.Right; break;
                    case "both": justification.Val = JustificationValues.Both; break; // Justify
                    default: justification.Val = JustificationValues.Left; break;
                }
                pPr.Append(justification);
            }

            // Spacing
            if (pConfig.SpacingAfter.HasValue)
            {
                pPr.Append(new SpacingBetweenLines() { After = pConfig.SpacingAfter.Value.ToString() });
            }
        }
        style.Append(pPr);

        // Run Properties (Font)
        var rPr = new StyleRunProperties();
        if (config.Properties?.Run != null)
        {
            var rConfig = config.Properties.Run;

            if (rConfig.Bold == true) rPr.Append(new Bold());
            if (rConfig.Italic == true) rPr.Append(new Italic());
            
            // Underline support
            if (!string.IsNullOrEmpty(rConfig.Underline))
            {
                var underlineValue = rConfig.Underline.ToLower();
                if (underlineValue != "none")
                {
                    var underline = new Underline();
                    switch (underlineValue)
                    {
                        case "single":
                            underline.Val = UnderlineValues.Single;
                            break;
                        case "double":
                            underline.Val = UnderlineValues.Double;
                            break;
                        default:
                            underline.Val = UnderlineValues.Single;
                            break;
                    }
                    rPr.Append(underline);
                }
            }
            
            if (rConfig.FontSizeMembers.HasValue) 
            {
                 // OpenXML Size is in half-points. So 11pt = 22
                 rPr.Append(new FontSize() { Val = (rConfig.FontSizeMembers.Value * 2).ToString() });
            }
        }
        style.Append(rPr);

        return style;
    }
}
