namespace TemplateGen.Core.Services;

using System.Collections.Generic;
using System.Linq;
using TemplateGen.Core.Models;

public class SemanticValidator
{
    private static readonly string[] RequiredStyles = { "Normal", "Heading1", "Heading2", "Heading3" };

    public List<ValidationError> Validate(TemplateProfile profile)
    {
        var errors = new List<ValidationError>();

        if (profile.Styles != null)
        {
            errors.AddRange(ValidateStyles(profile.Styles));
        }

        if (profile.Numbering != null && profile.Styles != null)
        {
            errors.AddRange(ValidateNumbering(profile.Numbering, profile.Styles));
        }

        return errors;
    }

    private List<ValidationError> ValidateStyles(StylesConfig styles)
    {
        var errors = new List<ValidationError>();
        var definedStyles = new HashSet<string>();

        // Collect all defined paragraph styles
        if (styles.ParagraphStyles != null)
        {
            foreach (var style in styles.ParagraphStyles)
            {
                if (!string.IsNullOrEmpty(style.StyleId))
                {
                    definedStyles.Add(style.StyleId);
                }
            }
        }

        // Check for required styles
        foreach (var requiredStyle in RequiredStyles)
        {
            if (!definedStyles.Contains(requiredStyle))
            {
                errors.Add(new ValidationError(
                    "error",
                    $"$.styles.paragraph_styles",
                    $"Required style '{requiredStyle}' is missing"
                ));
            }
        }

        // Validate based_on references
        if (styles.ParagraphStyles != null)
        {
            foreach (var style in styles.ParagraphStyles)
            {
                if (!string.IsNullOrEmpty(style.BasedOn) && !definedStyles.Contains(style.BasedOn))
                {
                    errors.Add(new ValidationError(
                        "error",
                        $"$.styles.paragraph_styles[?(@.style_id=='{style.StyleId}')].based_on",
                        $"Style '{style.StyleId}' references non-existent base style '{style.BasedOn}'"
                    ));
                }
            }
        }

        // Check for circular dependencies (simple check)
        if (styles.ParagraphStyles != null)
        {
            foreach (var style in styles.ParagraphStyles)
            {
                if (HasCircularDependency(style, styles.ParagraphStyles))
                {
                    errors.Add(new ValidationError(
                        "error",
                        $"$.styles.paragraph_styles[?(@.style_id=='{style.StyleId}')]",
                        $"Style '{style.StyleId}' has circular dependency in based_on chain"
                    ));
                }
            }
        }

        return errors;
    }

    private bool HasCircularDependency(ParagraphStyleConfig style, List<ParagraphStyleConfig> allStyles)
    {
        var visited = new HashSet<string>();
        var current = style.StyleId;

        while (!string.IsNullOrEmpty(current))
        {
            if (!visited.Add(current))
            {
                return true; // Circular dependency detected
            }

            var currentStyle = allStyles.FirstOrDefault(s => s.StyleId == current);
            if (currentStyle == null) break;

            current = currentStyle.BasedOn;
        }

        return false;
    }

    private List<ValidationError> ValidateNumbering(NumberingConfig numbering, StylesConfig styles)
    {
        var errors = new List<ValidationError>();
        var definedStyles = new HashSet<string>();

        if (styles.ParagraphStyles != null)
        {
            foreach (var style in styles.ParagraphStyles)
            {
                if (!string.IsNullOrEmpty(style.StyleId))
                {
                    definedStyles.Add(style.StyleId);
                }
            }
        }

        // Validate heading numbering style bindings
        if (numbering.HeadingNumbering?.StyleBindings != null)
        {
            foreach (var binding in numbering.HeadingNumbering.StyleBindings)
            {
                if (!string.IsNullOrEmpty(binding.StyleId) && !definedStyles.Contains(binding.StyleId))
                {
                    errors.Add(new ValidationError(
                        "error",
                        $"$.numbering.heading_numbering.style_bindings[?(@.style_id=='{binding.StyleId}')]",
                        $"Numbering references non-existent style '{binding.StyleId}'"
                    ));
                }

                // Validate level is in valid range (0-8)
                if (binding.LevelIndex < 0 || binding.LevelIndex > 8)
                {
                    errors.Add(new ValidationError(
                        "error",
                        $"$.numbering.heading_numbering.style_bindings[?(@.style_id=='{binding.StyleId}')].level_index",
                        $"Numbering level {binding.LevelIndex} is out of valid range (0-8)"
                    ));
                }
            }
        }

        return errors;
    }
}
