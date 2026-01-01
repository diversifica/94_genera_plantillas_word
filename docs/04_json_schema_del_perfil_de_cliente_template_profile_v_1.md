# JSON Schema del Perfil de Cliente (Template Profile)

## Sistema Generador de Plantillas Word (OpenXML)

---

## 1. Propósito del documento

Este documento define el **esquema formal (JSON Schema)** del Perfil de Cliente, utilizado para validar de forma automática la configuración declarativa que alimenta el motor generador de plantillas Word.

Objetivos:
- Validación determinista previa a la generación
- Contrato estable entre perfiles y motor
- Extensibilidad controlada sin romper compatibilidad

---

## 2. Convenciones

### 2.1 Versión del esquema

- `schema_version`: versión del esquema (semver)
- `profile_version`: versión del perfil del cliente (semver)

### 2.2 Unidades

Para evitar ambigüedad:
- Longitudes de layout (márgenes, sangrías, tab stops): **twips** (1/20 de punto)
- Tamaños de fuente: **points** (número decimal)
- Interlineado: `line_spacing` como:
  - `type: "multiple"` y `value` (ej. 1.15)
  - o `type: "exact"` y `value_twips`

### 2.3 Identificadores

- `*_id`: identificadores estables, únicos dentro del documento
- `style_id`: identificador interno del estilo (no depende del idioma)
- `style_name`: nombre mostrado en Word (puede ser localizado)

### 2.4 Extensibilidad

Se permite extensión mediante:
- Campo `extensions` a nivel raíz y en bloques principales
- El motor puede ignorar extensiones desconocidas

---

## 3. Reglas semánticas fuera del Schema (obligatorias)

El JSON Schema valida estructura y tipos. Además, el motor debe aplicar validaciones semánticas (definidas en el documento de Requisitos y Modelo de Dominio), por ejemplo:
- Detección de ciclos de herencia entre estilos
- Existencia del estilo base (Normal)
- Mapeo completo nivel -> estilo en numeración de títulos
- Referencias a `section_id` válidas en headers/footers
- Existencia de assets referenciados

---

## 4. JSON Schema (borrador v1)

```json
{
  "$schema": "https://json-schema.org/draft/2020-12/schema",
  "$id": "https://example.local/schemas/template-profile.schema.json",
  "title": "Template Profile",
  "type": "object",
  "additionalProperties": false,
  "required": [
    "schema_version",
    "metadata",
    "document",
    "fonts",
    "styles",
    "numbering",
    "sections",
    "headers_footers",
    "toc",
    "assets",
    "rules"
  ],
  "properties": {
    "schema_version": {
      "type": "string",
      "pattern": "^\\d+\\.\\d+\\.\\d+(-[0-9A-Za-z.-]+)?$"
    },
    "metadata": { "$ref": "#/$defs/metadata" },
    "document": { "$ref": "#/$defs/document" },
    "fonts": { "$ref": "#/$defs/fonts" },
    "styles": { "$ref": "#/$defs/styles" },
    "numbering": { "$ref": "#/$defs/numbering" },
    "sections": { "$ref": "#/$defs/sections" },
    "headers_footers": { "$ref": "#/$defs/headers_footers" },
    "toc": { "$ref": "#/$defs/toc" },
    "assets": { "$ref": "#/$defs/assets" },
    "rules": { "$ref": "#/$defs/rules" },
    "extensions": {
      "type": "object",
      "additionalProperties": true,
      "description": "Bloque libre para extensiones futuras. El motor puede ignorar claves desconocidas."
    }
  },
  "$defs": {
    "iso_datetime": {
      "type": "string",
      "pattern": "^\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}(\\.\\d+)?(Z|[+-]\\d{2}:\\d{2})$"
    },
    "semver": {
      "type": "string",
      "pattern": "^\\d+\\.\\d+\\.\\d+(-[0-9A-Za-z.-]+)?$"
    },
    "id": {
      "type": "string",
      "minLength": 1,
      "maxLength": 80,
      "pattern": "^[A-Za-z0-9._-]+$"
    },
    "twips": {
      "type": "integer",
      "minimum": 0,
      "maximum": 28800,
      "description": "Unidad en twips (1/20 punto)."
    },
    "points": {
      "type": "number",
      "minimum": 1,
      "maximum": 200
    },
    "color_hex": {
      "type": "string",
      "pattern": "^#([0-9A-Fa-f]{6})$"
    },

    "metadata": {
      "type": "object",
      "additionalProperties": false,
      "required": [
        "client_id",
        "client_name",
        "profile_version",
        "created_at",
        "updated_at",
        "author"
      ],
      "properties": {
        "client_id": { "$ref": "#/$defs/id" },
        "client_name": { "type": "string", "minLength": 1, "maxLength": 200 },
        "profile_version": { "$ref": "#/$defs/semver" },
        "created_at": { "$ref": "#/$defs/iso_datetime" },
        "updated_at": { "$ref": "#/$defs/iso_datetime" },
        "author": { "type": "string", "minLength": 1, "maxLength": 200 },
        "description": { "type": "string", "maxLength": 2000 },
        "tags": {
          "type": "array",
          "items": { "type": "string", "minLength": 1, "maxLength": 50 },
          "maxItems": 50
        },
        "extensions": {
          "type": "object",
          "additionalProperties": true
        }
      }
    },

    "document": {
      "type": "object",
      "additionalProperties": false,
      "required": ["page_size", "orientation", "margins", "language", "default_tab_stop_twips"],
      "properties": {
        "page_size": {
          "type": "string",
          "enum": ["A4", "Letter", "Legal", "A3", "A5"]
        },
        "orientation": { "type": "string", "enum": ["portrait", "landscape"] },
        "margins": {
          "type": "object",
          "additionalProperties": false,
          "required": ["top_twips", "bottom_twips", "left_twips", "right_twips"],
          "properties": {
            "top_twips": { "$ref": "#/$defs/twips" },
            "bottom_twips": { "$ref": "#/$defs/twips" },
            "left_twips": { "$ref": "#/$defs/twips" },
            "right_twips": { "$ref": "#/$defs/twips" }
          }
        },
        "language": {
          "type": "string",
          "pattern": "^[a-z]{2}-[A-Z]{2}$",
          "examples": ["es-ES", "en-US"]
        },
        "default_tab_stop_twips": { "$ref": "#/$defs/twips" },
        "compatibility": {
          "type": "object",
          "additionalProperties": false,
          "properties": {
            "target_word": { "type": "string", "enum": ["modern", "2016+", "2019+", "365"] },
            "strict_openxml": { "type": "boolean" }
          }
        },
        "extensions": {
          "type": "object",
          "additionalProperties": true
        }
      }
    },

    "fonts": {
      "type": "object",
      "additionalProperties": false,
      "required": ["body", "headings", "monospace"],
      "properties": {
        "body": { "$ref": "#/$defs/font" },
        "headings": { "$ref": "#/$defs/font" },
        "monospace": { "$ref": "#/$defs/font" },
        "extensions": {
          "type": "object",
          "additionalProperties": true
        }
      }
    },

    "font": {
      "type": "object",
      "additionalProperties": false,
      "required": ["font_name"],
      "properties": {
        "font_name": { "type": "string", "minLength": 1, "maxLength": 120 },
        "fallback_fonts": {
          "type": "array",
          "items": { "type": "string", "minLength": 1, "maxLength": 120 },
          "maxItems": 10
        }
      }
    },

    "styles": {
      "type": "object",
      "additionalProperties": false,
      "required": ["paragraph_styles", "character_styles"],
      "properties": {
        "paragraph_styles": {
          "type": "array",
          "minItems": 1,
          "items": { "$ref": "#/$defs/paragraph_style" }
        },
        "character_styles": {
          "type": "array",
          "items": { "$ref": "#/$defs/character_style" }
        },
        "extensions": {
          "type": "object",
          "additionalProperties": true
        }
      }
    },

    "paragraph_style": {
      "type": "object",
      "additionalProperties": false,
      "required": ["style_id", "style_name", "type", "properties"],
      "properties": {
        "style_id": { "$ref": "#/$defs/id" },
        "style_name": { "type": "string", "minLength": 1, "maxLength": 120 },
        "type": { "const": "paragraph" },
        "based_on": { "$ref": "#/$defs/id" },
        "properties": { "$ref": "#/$defs/paragraph_style_properties" },
        "extensions": {
          "type": "object",
          "additionalProperties": true
        }
      }
    },

    "paragraph_style_properties": {
      "type": "object",
      "additionalProperties": false,
      "required": ["run", "paragraph"],
      "properties": {
        "run": { "$ref": "#/$defs/run_properties" },
        "paragraph": { "$ref": "#/$defs/paragraph_properties" },
        "pagination": { "$ref": "#/$defs/pagination_properties" }
      }
    },

    "character_style": {
      "type": "object",
      "additionalProperties": false,
      "required": ["style_id", "style_name", "type", "run"],
      "properties": {
        "style_id": { "$ref": "#/$defs/id" },
        "style_name": { "type": "string", "minLength": 1, "maxLength": 120 },
        "type": { "const": "character" },
        "run": { "$ref": "#/$defs/run_properties" },
        "extensions": {
          "type": "object",
          "additionalProperties": true
        }
      }
    },

    "run_properties": {
      "type": "object",
      "additionalProperties": false,
      "properties": {
        "font_role": { "type": "string", "enum": ["body", "headings", "monospace"] },
        "font_name": { "type": "string", "minLength": 1, "maxLength": 120 },
        "font_size_points": { "$ref": "#/$defs/points" },
        "bold": { "type": "boolean" },
        "italic": { "type": "boolean" },
        "underline": { "type": "string", "enum": ["none", "single" ] },
        "color": { "$ref": "#/$defs/color_hex" },
        "caps": { "type": "boolean" }
      },
      "anyOf": [
        { "required": ["font_role"] },
        { "required": ["font_name"] }
      ]
    },

    "paragraph_properties": {
      "type": "object",
      "additionalProperties": false,
      "properties": {
        "alignment": { "type": "string", "enum": ["left", "right", "center", "justify"] },
        "indentation": { "$ref": "#/$defs/indentation" },
        "spacing_before_twips": { "$ref": "#/$defs/twips" },
        "spacing_after_twips": { "$ref": "#/$defs/twips" },
        "line_spacing": { "$ref": "#/$defs/line_spacing" },
        "outline_level": {
          "type": "integer",
          "minimum": 0,
          "maximum": 9,
          "description": "Nivel de esquema (para títulos)."
        }
      }
    },

    "indentation": {
      "type": "object",
      "additionalProperties": false,
      "properties": {
        "left_twips": { "$ref": "#/$defs/twips" },
        "right_twips": { "$ref": "#/$defs/twips" },
        "first_line_twips": { "$ref": "#/$defs/twips" },
        "hanging_twips": { "$ref": "#/$defs/twips" }
      }
    },

    "line_spacing": {
      "type": "object",
      "additionalProperties": false,
      "required": ["type"],
      "properties": {
        "type": { "type": "string", "enum": ["single", "multiple", "exact"] },
        "value": { "type": "number", "minimum": 0.8, "maximum": 10 },
        "value_twips": { "$ref": "#/$defs/twips" }
      },
      "allOf": [
        {
          "if": { "properties": { "type": { "const": "multiple" } } },
          "then": { "required": ["value"] }
        },
        {
          "if": { "properties": { "type": { "const": "exact" } } },
          "then": { "required": ["value_twips"] }
        }
      ]
    },

    "pagination_properties": {
      "type": "object",
      "additionalProperties": false,
      "properties": {
        "keep_with_next": { "type": "boolean" },
        "keep_lines": { "type": "boolean" },
        "page_break_before": { "type": "boolean" },
        "widow_control": { "type": "boolean" }
      }
    },

    "numbering": {
      "type": "object",
      "additionalProperties": false,
      "required": ["heading_numbering", "list_numbering"],
      "properties": {
        "heading_numbering": { "$ref": "#/$defs/heading_numbering" },
        "list_numbering": { "$ref": "#/$defs/list_numbering" },
        "extensions": {
          "type": "object",
          "additionalProperties": true
        }
      }
    },

    "heading_numbering": {
      "type": "object",
      "additionalProperties": false,
      "required": ["scheme_id", "levels", "style_bindings"],
      "properties": {
        "scheme_id": { "$ref": "#/$defs/id" },
        "restart_on_section": { "type": "boolean" },
        "levels": {
          "type": "array",
          "minItems": 1,
          "maxItems": 9,
          "items": { "$ref": "#/$defs/heading_level" }
        },
        "style_bindings": {
          "type": "array",
          "minItems": 1,
          "items": { "$ref": "#/$defs/heading_style_binding" }
        }
      }
    },

    "heading_level": {
      "type": "object",
      "additionalProperties": false,
      "required": ["level_index", "number_format", "level_text_pattern", "start_at", "indentation"],
      "properties": {
        "level_index": { "type": "integer", "minimum": 0, "maximum": 8 },
        "number_format": { "type": "string", "enum": ["decimal", "upperRoman", "lowerRoman", "upperLetter", "lowerLetter"] },
        "level_text_pattern": { "type": "string", "minLength": 1, "maxLength": 40 },
        "start_at": { "type": "integer", "minimum": 1, "maximum": 9999 },
        "indentation": { "$ref": "#/$defs/indentation" }
      }
    },

    "heading_style_binding": {
      "type": "object",
      "additionalProperties": false,
      "required": ["level_index", "style_id"],
      "properties": {
        "level_index": { "type": "integer", "minimum": 0, "maximum": 8 },
        "style_id": { "$ref": "#/$defs/id" }
      }
    },

    "list_numbering": {
      "type": "object",
      "additionalProperties": false,
      "required": ["numbered_lists", "bullet_lists"],
      "properties": {
        "numbered_lists": {
          "type": "array",
          "items": { "$ref": "#/$defs/list_scheme" }
        },
        "bullet_lists": {
          "type": "array",
          "items": { "$ref": "#/$defs/list_scheme" }
        }
      }
    },

    "list_scheme": {
      "type": "object",
      "additionalProperties": false,
      "required": ["scheme_id", "level_index", "indentation"],
      "properties": {
        "scheme_id": { "$ref": "#/$defs/id" },
        "level_index": { "type": "integer", "minimum": 0, "maximum": 8 },
        "restart_behavior": { "type": "string", "enum": ["continue", "restart_on_style", "restart_on_section"] },
        "indentation": { "$ref": "#/$defs/indentation" }
      }
    },

    "sections": {
      "type": "object",
      "additionalProperties": false,
      "required": ["items"],
      "properties": {
        "items": {
          "type": "array",
          "minItems": 1,
          "items": { "$ref": "#/$defs/section" }
        },
        "extensions": {
          "type": "object",
          "additionalProperties": true
        }
      }
    },

    "section": {
      "type": "object",
      "additionalProperties": false,
      "required": ["section_id", "type", "page_numbering"],
      "properties": {
        "section_id": { "$ref": "#/$defs/id" },
        "type": { "type": "string", "enum": ["cover", "toc", "body", "annexes", "custom"] },
        "page_numbering": { "$ref": "#/$defs/page_numbering" },
        "different_first_page": { "type": "boolean" },
        "layout_overrides": {
          "type": "object",
          "additionalProperties": false,
          "properties": {
            "orientation": { "type": "string", "enum": ["portrait", "landscape"] },
            "margins": { "$ref": "#/$defs/document/properties/margins" }
          }
        },
        "extensions": {
          "type": "object",
          "additionalProperties": true
        }
      }
    },

    "page_numbering": {
      "type": "object",
      "additionalProperties": false,
      "required": ["mode"],
      "properties": {
        "mode": { "type": "string", "enum": ["none", "roman", "arabic"] },
        "start_at": { "type": "integer", "minimum": 1, "maximum": 9999 }
      }
    },

    "headers_footers": {
      "type": "object",
      "additionalProperties": false,
      "required": ["items"],
      "properties": {
        "items": {
          "type": "array",
          "items": { "$ref": "#/$defs/header_footer_item" }
        },
        "extensions": {
          "type": "object",
          "additionalProperties": true
        }
      }
    },

    "header_footer_item": {
      "type": "object",
      "additionalProperties": false,
      "required": ["section_id", "header", "footer"],
      "properties": {
        "section_id": { "$ref": "#/$defs/id" },
        "header": { "$ref": "#/$defs/header_footer_definition" },
        "footer": { "$ref": "#/$defs/header_footer_definition" }
      }
    },

    "header_footer_definition": {
      "type": "object",
      "additionalProperties": false,
      "properties": {
        "text_left": { "type": "string", "maxLength": 500 },
        "text_center": { "type": "string", "maxLength": 500 },
        "text_right": { "type": "string", "maxLength": 500 },
        "include_page_number": { "type": "boolean" },
        "page_number_format": { "type": "string", "enum": ["roman", "arabic"] },
        "show_on_first_page": { "type": "boolean" },
        "logo_asset_id": { "$ref": "#/$defs/id" }
      }
    },

    "toc": {
      "type": "object",
      "additionalProperties": false,
      "required": ["enabled", "levels"],
      "properties": {
        "enabled": { "type": "boolean" },
        "levels": { "type": "integer", "minimum": 1, "maximum": 9 },
        "include_annexes": { "type": "boolean" },
        "toc_title": { "type": "string", "maxLength": 200 },
        "style_mapping": {
          "type": "array",
          "items": { "$ref": "#/$defs/toc_style_mapping" }
        },
        "extensions": {
          "type": "object",
          "additionalProperties": true
        }
      }
    },

    "toc_style_mapping": {
      "type": "object",
      "additionalProperties": false,
      "required": ["outline_level", "style_id"],
      "properties": {
        "outline_level": { "type": "integer", "minimum": 0, "maximum": 8 },
        "style_id": { "$ref": "#/$defs/id" }
      }
    },

    "assets": {
      "type": "object",
      "additionalProperties": false,
      "required": ["items"],
      "properties": {
        "items": {
          "type": "array",
          "items": { "$ref": "#/$defs/asset" }
        },
        "extensions": {
          "type": "object",
          "additionalProperties": true
        }
      }
    },

    "asset": {
      "type": "object",
      "additionalProperties": false,
      "required": ["asset_id", "relative_path", "type"],
      "properties": {
        "asset_id": { "$ref": "#/$defs/id" },
        "relative_path": { "type": "string", "minLength": 1, "maxLength": 260 },
        "type": { "type": "string", "enum": ["image", "logo", "other"] },
        "usage": { "type": "string", "maxLength": 80 }
      }
    },

    "rules": {
      "type": "object",
      "additionalProperties": false,
      "required": ["forbid_manual_formatting", "enforce_styles_only"],
      "properties": {
        "forbid_manual_formatting": { "type": "boolean" },
        "enforce_styles_only": { "type": "boolean" },
        "auto_map_bold_to_strong_emphasis": { "type": "boolean" },
        "auto_map_italic_to_emphasis": { "type": "boolean" },
        "extensions": {
          "type": "object",
          "additionalProperties": true
        }
      }
    }
  }
}
```

---

## 5. Estilos obligatorios (catálogo mínimo)

Este esquema no fuerza por sí mismo la presencia de estilos concretos por `style_id` (porque los nombres pueden variar). La obligatoriedad se valida con reglas semánticas.

Se recomienda establecer una convención de `style_id` fija para el motor, por ejemplo:

- `Normal`
- `Title1`
- `Title2`
- `Title3`
- `BulletList`
- `NumberedList`
- `Quote`
- `Note`
- `Example`
- `CodeBlock`
- `FigureCaption`

Y para carácter:

- `Emphasis`
- `StrongEmphasis`
- `Term`
- `InlineCode`

---

## 6. Notas de implementación

1. El motor debe aplicar una validación adicional que garantice que `Normal` existe y que el resto de estilos obligatorios están presentes.
2. `based_on` debe resolverse; ciclos deben bloquear la generación.
3. Las referencias cruzadas (`section_id`, `logo_asset_id`) deben verificarse.
4. Las unidades (twips/puntos) deben traducirse a OpenXML sin heurísticas.

---

## 7. Estado del documento

Este documento define el JSON Schema inicial del Perfil de Cliente. Habilita el siguiente paso: diseño de arquitectura de implementación y el conjunto de validaciones semánticas (validador) que complementan al schema.

