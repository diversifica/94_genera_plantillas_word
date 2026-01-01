namespace TemplateGen.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TemplateGen.Core.Models;

/// <summary>
/// Generates OpenXML numbering definitions for document templates.
/// Note: Uses OpenXmlUnknownElement for some elements due to limitations
/// in DocumentFormat.OpenXml library for setting certain properties.
/// </summary>
public class NumberingDefinitionsGenerator
{
    // OpenXML namespace constants
    private const string WordNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
    private const string WordPrefix = "w";
    
    // Element name constants
    private const string AbstractNumElement = "abstractNum";
    private const string NumElement = "num";
    private const string AbstractNumIdElement = "abstractNumId";
    private const string NumIdElement = "numId";

    public Numbering GenerateNumbering(NumberingConfig config)
    {
        ValidateConfig(config);
        
        var numbering = new Numbering();
        var sb = new System.Text.StringBuilder();

        int abstractNumIdCounter = 1;

        if (config.HeadingNumbering != null)
        {
            sb.Append(CreateHeadingAbstractNumXml(config.HeadingNumbering, abstractNumIdCounter));
            
            // Create Num XML
            sb.Append($@"<w:num w:numId=""{abstractNumIdCounter}"" xmlns:w=""{WordNamespace}""><w:abstractNumId w:val=""{abstractNumIdCounter}"" /></w:num>");
            
            abstractNumIdCounter++;
        }

        if (config.ListNumbering != null)
        {
            if (config.ListNumbering.NumberedLists != null)
            {
                foreach (var listConfig in config.ListNumbering.NumberedLists)
                {
                     sb.Append(CreateListAbstractNumXml(listConfig, abstractNumIdCounter, true));
                     sb.Append($@"<w:num w:numId=""{abstractNumIdCounter}"" xmlns:w=""{W_NAMESPACE}""><w:abstractNumId w:val=""{abstractNumIdCounter}"" /></w:num>");
                     abstractNumIdCounter++;
                }
            }

            if (config.ListNumbering.BulletLists != null)
            {
                foreach (var listConfig in config.ListNumbering.BulletLists)
                {
                     sb.Append(CreateListAbstractNumXml(listConfig, abstractNumIdCounter, false));
                     sb.Append($@"<w:num w:numId=""{abstractNumIdCounter}"" xmlns:w=""{W_NAMESPACE}""><w:abstractNumId w:val=""{abstractNumIdCounter}"" /></w:num>");
                     sb.Append($@"<w:num w:numId=""{abstractNumIdCounter}"" xmlns:w=""{WordNamespace}""><w:abstractNumId w:val=""{abstractNumIdCounter}"" /></w:num>");
                     abstractNumIdCounter++;
                }
            }
        }

        numbering.InnerXml = sb.ToString();
        return numbering;
    }
    
    /// <summary>
    /// Validates the numbering configuration before processing.
    /// </summary>
    private void ValidateConfig(NumberingConfig config)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config), "Numbering configuration cannot be null");
        }
        
        if (config.HeadingNumbering != null)
        {
            if (config.HeadingNumbering.Levels == null || !config.HeadingNumbering.Levels.Any())
            {
                throw new ArgumentException("Heading numbering must have at least one level", nameof(config));
            }
            
            foreach (var level in config.HeadingNumbering.Levels)
            {
                if (level.LevelIndex < 0 || level.LevelIndex > 8)
                {
                    throw new ArgumentOutOfRangeException(nameof(config), 
                        $"Numbering level {level.LevelIndex} is out of valid range (0-8)");
                }
            }
        }
    }

    private string CreateHeadingAbstractNumXml(HeadingNumberingConfig config, int id)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append($@"<w:abstractNum w:abstractNumId=""{id}"" xmlns:w=""{WordNamespace}"">");
        sb.Append(@"<w:multiLevelType w:val=""multilevel"" />");

        foreach (var levelConfig in config.Levels)
        {
            sb.Append($@"<w:lvl w:ilvl=""{levelConfig.LevelIndex}"">");
            sb.Append($@"<w:start w:val=""{levelConfig.StartAt}"" />");
            
            var formatVal = "decimal"; // default
             try 
            {
                var parsed = Enum.Parse(typeof(NumberFormatValues), levelConfig.NumberFormat, true);
                if (parsed != null) formatVal = parsed.ToString().ToLower(); // enum to string
            }
            catch { }
            
            sb.Append($@"<w:numFmt w:val=""{formatVal}"" />");
            sb.Append($@"<w:lvlText w:val=""{levelConfig.LevelTextPattern}"" />");
            sb.Append(@"<w:lvlJc w:val=""left"" />");

            if (levelConfig.Indentation != null)
            {
                sb.Append("<w:pPr><w:ind");
                sb.Append($@" w:left=""{levelConfig.Indentation.LeftTwips}""");
                if (levelConfig.Indentation.HangingTwips.HasValue)
                     sb.Append($@" w:hanging=""{levelConfig.Indentation.HangingTwips}""");
                sb.Append(" /></w:pPr>");
            }

            if (config.StyleBindings != null)
            {
                var binding = config.StyleBindings.FirstOrDefault(b => b.LevelIndex == levelConfig.LevelIndex);
                if (binding != null)
                {
                    sb.Append($@"<w:pStyle w:val=""{binding.StyleId}"" />");
                }
            }

            sb.Append("</w:lvl>");
        }

        sb.Append("</w:abstractNum>");
        return sb.ToString();
    }

    private string CreateListAbstractNumXml(ListConfig config, int id, bool isNumbered)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append($@"<w:abstractNum w:abstractNumId=""{id}"" xmlns:w=""{W_NAMESPACE}"">");
        sb.Append(@"<w:multiLevelType w:val=""hybridMultilevel"" />");
        
        sb.Append(@"<w:lvl w:ilvl=""0"">");
        sb.Append(@"<w:start w:val=""1"" />");
        
        if (isNumbered)
        {
            sb.Append(@"<w:numFmt w:val=""decimal"" />");
            sb.Append(@"<w:lvlText w:val=""%1."" />");
        }
        else
        {
            sb.Append(@"<w:numFmt w:val=""bullet"" />");
            sb.Append(@"<w:lvlText w:val=""•"" />");
        }
        sb.Append(@"<w:lvlJc w:val=""left"" />");

         if (config.Indentation != null)
            {
                sb.Append("<w:pPr><w:ind");
                sb.Append($@" w:left=""{config.Indentation.LeftTwips}""");
                if (config.Indentation.HangingTwips.HasValue)
                     sb.Append($@" w:hanging=""{config.Indentation.HangingTwips}""");
                sb.Append(" /></w:pPr>");
            }

        sb.Append("</w:lvl>");
        sb.Append("</w:abstractNum>");
        return sb.ToString(); 
    }
}
