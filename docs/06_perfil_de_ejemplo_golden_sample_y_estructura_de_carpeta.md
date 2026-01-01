# Perfil de Ejemplo (Golden Sample) y Estructura de Carpeta

## Sistema Generador de Plantillas Word (OpenXML)

---

## 1. Propósito del documento

Este documento proporciona un **perfil de cliente de ejemplo completo** (golden sample) y la **estructura de carpetas** recomendada para ejecutar la primera PoC del motor generador.

Objetivos:
- Contar con un perfil `profile.json` listo para validar con el JSON Schema
- Definir un conjunto mínimo pero completo de estilos, numeración y secciones
- Proveer un caso base estable para pruebas de integración (golden tests)

---

## 2. Estructura de carpetas (por cliente)

Se recomienda la siguiente estructura:

- `profiles/<client_id>/`
  - `profile.json`
  - `assets/`
    - `logo.png`
    - `cover_logo.png` (opcional)
  - `output/`
    - `template.dotx`
    - `sample.docx`
    - `report.json`
    - `report.md`
  - `changelog.md`

Notas:
- `output/` puede limpiarse en cada ejecución si se desea.
- Los assets deben referenciarse siempre por ruta relativa dentro de `assets/`.

---

## 3. Perfil de ejemplo: `profiles/acme_consulting/profile.json`

### 3.1 Descripción

Este perfil ejemplifica:
- Documento A4 vertical
- Márgenes estándar
- Fuentes por rol (cuerpo, títulos, monoespaciada)
- Catálogo mínimo de estilos (párrafo y carácter)
- Numeración multinivel para títulos (1, 1.1, 1.1.1)
- Secciones: portada, índice, cuerpo, anexos
- Encabezado y pie con numeración arábiga en el cuerpo
- TOC habilitado
- Un logo en cabecera

### 3.2 Contenido JSON

```json
{
  "schema_version": "1.0.0",
  "metadata": {
    "client_id": "acme_consulting",
    "client_name": "ACME Consulting",
    "profile_version": "1.0.0",
    "created_at": "2026-01-01T00:00:00+01:00",
    "updated_at": "2026-01-01T00:00:00+01:00",
    "author": "Antonio",
    "description": "Perfil golden sample para validar el motor OpenXML.",
    "tags": ["golden", "poc", "a4", "es-ES"]
  },
  "document": {
    "page_size": "A4",
    "orientation": "portrait",
    "margins": {
      "top_twips": 1440,
      "bottom_twips": 1440,
      "left_twips": 1440,
      "right_twips": 1440
    },
    "language": "es-ES",
    "default_tab_stop_twips": 720,
    "compatibility": {
      "target_word": "365",
      "strict_openxml": true
    }
  },
  "fonts": {
    "body": {
      "font_name": "Calibri",
      "fallback_fonts": ["Arial", "Liberation Sans"]
    },
    "headings": {
      "font_name": "Calibri",
      "fallback_fonts": ["Arial", "Liberation Sans"]
    },
    "monospace": {
      "font_name": "Consolas",
      "fallback_fonts": ["Courier New", "Liberation Mono"]
    }
  },
  "styles": {
    "paragraph_styles": [
      {
        "style_id": "Normal",
        "style_name": "Normal",
        "type": "paragraph",
        "properties": {
          "run": {
            "font_role": "body",
            "font_size_points": 11,
            "bold": false,
            "italic": false,
            "underline": "none",
            "caps": false
          },
          "paragraph": {
            "alignment": "justify",
            "spacing_before_twips": 0,
            "spacing_after_twips": 120,
            "line_spacing": { "type": "multiple", "value": 1.15 }
          },
          "pagination": {
            "widow_control": true
          }
        }
      },
      {
        "style_id": "Title1",
        "style_name": "Título 1",
        "type": "paragraph",
        "based_on": "Normal",
        "properties": {
          "run": {
            "font_role": "headings",
            "font_size_points": 18,
            "bold": true
          },
          "paragraph": {
            "alignment": "left",
            "spacing_before_twips": 480,
            "spacing_after_twips": 240,
            "line_spacing": { "type": "multiple", "value": 1.0 },
            "outline_level": 0
          },
          "pagination": {
            "keep_with_next": true,
            "page_break_before": true
          }
        }
      },
      {
        "style_id": "Title2",
        "style_name": "Título 2",
        "type": "paragraph",
        "based_on": "Normal",
        "properties": {
          "run": {
            "font_role": "headings",
            "font_size_points": 14,
            "bold": true
          },
          "paragraph": {
            "alignment": "left",
            "spacing_before_twips": 360,
            "spacing_after_twips": 180,
            "line_spacing": { "type": "multiple", "value": 1.0 },
            "outline_level": 1
          },
          "pagination": {
            "keep_with_next": true
          }
        }
      },
      {
        "style_id": "Title3",
        "style_name": "Título 3",
        "type": "paragraph",
        "based_on": "Normal",
        "properties": {
          "run": {
            "font_role": "headings",
            "font_size_points": 12,
            "bold": true
          },
          "paragraph": {
            "alignment": "left",
            "spacing_before_twips": 240,
            "spacing_after_twips": 120,
            "line_spacing": { "type": "multiple", "value": 1.0 },
            "outline_level": 2
          },
          "pagination": {
            "keep_with_next": true
          }
        }
      },
      {
        "style_id": "BulletList",
        "style_name": "Lista con viñetas",
        "type": "paragraph",
        "based_on": "Normal",
        "properties": {
          "run": {
            "font_role": "body",
            "font_size_points": 11
          },
          "paragraph": {
            "alignment": "left",
            "indentation": { "left_twips": 720, "hanging_twips": 360 },
            "spacing_before_twips": 0,
            "spacing_after_twips": 120,
            "line_spacing": { "type": "multiple", "value": 1.15 }
          }
        }
      },
      {
        "style_id": "NumberedList",
        "style_name": "Lista numerada",
        "type": "paragraph",
        "based_on": "Normal",
        "properties": {
          "run": {
            "font_role": "body",
            "font_size_points": 11
          },
          "paragraph": {
            "alignment": "left",
            "indentation": { "left_twips": 720, "hanging_twips": 360 },
            "spacing_before_twips": 0,
            "spacing_after_twips": 120,
            "line_spacing": { "type": "multiple", "value": 1.15 }
          }
        }
      },
      {
        "style_id": "Quote",
        "style_name": "Cita",
        "type": "paragraph",
        "based_on": "Normal",
        "properties": {
          "run": {
            "font_role": "body",
            "font_size_points": 11,
            "italic": true
          },
          "paragraph": {
            "alignment": "left",
            "indentation": { "left_twips": 720, "right_twips": 360 },
            "spacing_before_twips": 120,
            "spacing_after_twips": 120,
            "line_spacing": { "type": "multiple", "value": 1.15 }
          }
        }
      },
      {
        "style_id": "Note",
        "style_name": "Nota",
        "type": "paragraph",
        "based_on": "Normal",
        "properties": {
          "run": {
            "font_role": "body",
            "font_size_points": 11,
            "bold": true
          },
          "paragraph": {
            "alignment": "left",
            "indentation": { "left_twips": 360 },
            "spacing_before_twips": 120,
            "spacing_after_twips": 120,
            "line_spacing": { "type": "multiple", "value": 1.15 }
          }
        }
      },
      {
        "style_id": "Example",
        "style_name": "Ejemplo",
        "type": "paragraph",
        "based_on": "Normal",
        "properties": {
          "run": {
            "font_role": "body",
            "font_size_points": 11
          },
          "paragraph": {
            "alignment": "left",
            "indentation": { "left_twips": 360 },
            "spacing_before_twips": 120,
            "spacing_after_twips": 120,
            "line_spacing": { "type": "multiple", "value": 1.15 }
          }
        }
      },
      {
        "style_id": "CodeBlock",
        "style_name": "Código",
        "type": "paragraph",
        "based_on": "Normal",
        "properties": {
          "run": {
            "font_role": "monospace",
            "font_size_points": 10
          },
          "paragraph": {
            "alignment": "left",
            "indentation": { "left_twips": 360 },
            "spacing_before_twips": 120,
            "spacing_after_twips": 120,
            "line_spacing": { "type": "multiple", "value": 1.0 }
          }
        }
      },
      {
        "style_id": "FigureCaption",
        "style_name": "Pie de figura",
        "type": "paragraph",
        "based_on": "Normal",
        "properties": {
          "run": {
            "font_role": "body",
            "font_size_points": 10,
            "italic": true
          },
          "paragraph": {
            "alignment": "center",
            "spacing_before_twips": 120,
            "spacing_after_twips": 240,
            "line_spacing": { "type": "multiple", "value": 1.0 }
          }
        }
      }
    ],
    "character_styles": [
      {
        "style_id": "Emphasis",
        "style_name": "Énfasis",
        "type": "character",
        "run": { "font_role": "body", "italic": true }
      },
      {
        "style_id": "StrongEmphasis",
        "style_name": "Énfasis fuerte",
        "type": "character",
        "run": { "font_role": "body", "bold": true }
      },
      {
        "style_id": "Term",
        "style_name": "Término",
        "type": "character",
        "run": { "font_role": "body", "bold": true }
      },
      {
        "style_id": "InlineCode",
        "style_name": "Código en línea",
        "type": "character",
        "run": { "font_role": "monospace" }
      }
    ]
  },
  "numbering": {
    "heading_numbering": {
      "scheme_id": "HeadingsDecimal",
      "restart_on_section": false,
      "levels": [
        {
          "level_index": 0,
          "number_format": "decimal",
          "level_text_pattern": "%1.",
          "start_at": 1,
          "indentation": { "left_twips": 0, "hanging_twips": 360 }
        },
        {
          "level_index": 1,
          "number_format": "decimal",
          "level_text_pattern": "%1.%2.",
          "start_at": 1,
          "indentation": { "left_twips": 360, "hanging_twips": 360 }
        },
        {
          "level_index": 2,
          "number_format": "decimal",
          "level_text_pattern": "%1.%2.%3.",
          "start_at": 1,
          "indentation": { "left_twips": 720, "hanging_twips": 360 }
        }
      ],
      "style_bindings": [
        { "level_index": 0, "style_id": "Title1" },
        { "level_index": 1, "style_id": "Title2" },
        { "level_index": 2, "style_id": "Title3" }
      ]
    },
    "list_numbering": {
      "numbered_lists": [
        {
          "scheme_id": "ListDecimal",
          "level_index": 0,
          "restart_behavior": "restart_on_style",
          "indentation": { "left_twips": 720, "hanging_twips": 360 }
        }
      ],
      "bullet_lists": [
        {
          "scheme_id": "ListBullet",
          "level_index": 0,
          "restart_behavior": "continue",
          "indentation": { "left_twips": 720, "hanging_twips": 360 }
        }
      ]
    }
  },
  "sections": {
    "items": [
      {
        "section_id": "S_COVER",
        "type": "cover",
        "page_numbering": { "mode": "none" },
        "different_first_page": true
      },
      {
        "section_id": "S_TOC",
        "type": "toc",
        "page_numbering": { "mode": "roman", "start_at": 1 },
        "different_first_page": true
      },
      {
        "section_id": "S_BODY",
        "type": "body",
        "page_numbering": { "mode": "arabic", "start_at": 1 },
        "different_first_page": false
      },
      {
        "section_id": "S_ANNEX",
        "type": "annexes",
        "page_numbering": { "mode": "arabic" },
        "different_first_page": false
      }
    ]
  },
  "headers_footers": {
    "items": [
      {
        "section_id": "S_COVER",
        "header": { "text_center": "", "include_page_number": false },
        "footer": { "text_center": "", "include_page_number": false }
      },
      {
        "section_id": "S_TOC",
        "header": {
          "text_left": "ACME Consulting",
          "include_page_number": false,
          "logo_asset_id": "LOGO_MAIN"
        },
        "footer": {
          "include_page_number": true,
          "page_number_format": "roman"
        }
      },
      {
        "section_id": "S_BODY",
        "header": {
          "text_left": "ACME Consulting",
          "text_right": "Documento",
          "include_page_number": false,
          "logo_asset_id": "LOGO_MAIN"
        },
        "footer": {
          "text_left": "Confidencial",
          "include_page_number": true,
          "page_number_format": "arabic"
        }
      },
      {
        "section_id": "S_ANNEX",
        "header": {
          "text_left": "ACME Consulting",
          "text_right": "Anexos",
          "include_page_number": false,
          "logo_asset_id": "LOGO_MAIN"
        },
        "footer": {
          "include_page_number": true,
          "page_number_format": "arabic"
        }
      }
    ]
  },
  "toc": {
    "enabled": true,
    "levels": 3,
    "include_annexes": true,
    "toc_title": "Índice",
    "style_mapping": [
      { "outline_level": 0, "style_id": "Title1" },
      { "outline_level": 1, "style_id": "Title2" },
      { "outline_level": 2, "style_id": "Title3" }
    ]
  },
  "assets": {
    "items": [
      {
        "asset_id": "LOGO_MAIN",
        "relative_path": "assets/logo.png",
        "type": "logo",
        "usage": "header"
      }
    ]
  },
  "rules": {
    "forbid_manual_formatting": true,
    "enforce_styles_only": true,
    "auto_map_bold_to_strong_emphasis": true,
    "auto_map_italic_to_emphasis": true
  }
}
```

---

## 4. Requisitos de assets para el ejemplo

Para que el perfil funcione:

- Debe existir `profiles/acme_consulting/assets/logo.png`
- Tamaño recomendado del logo: ancho 300 a 600 px (PNG)
- Fondo preferible transparente

---

## 5. Criterios de validación del golden sample

Antes de ejecutar el render:

1. `profile.json` valida contra `schemas/template-profile.schema.json`
2. Validación semántica:
   - Existe `Normal`, `Title1`, `Title2`, `Title3`
   - `based_on` resoluble
   - `HeadingsDecimal` mapea niveles 0..2 a estilos
   - Secciones S_COVER, S_TOC, S_BODY, S_ANNEX existen
   - Headers/footers referencian secciones válidas
   - `LOGO_MAIN` existe y el archivo está presente

---

## 6. Validación visual (al abrir `sample.docx`)

Al abrir el documento de prueba en Word, debe verificarse:

- Portada sin numeración
- Índice con numeración romana (I, II, III) al pie
- Cuerpo con numeración arábiga (1, 2, 3) reiniciando en 1
- Títulos numerados correctamente:
  - Título 1: 1.
  - Título 2: 1.1.
  - Título 3: 1.1.1.
- Listas con sangría consistente
- Bloques de cita y nota con estilo diferenciado
- Código en bloque con fuente monoespaciada
- Cabecera con logo presente en TOC y Body (según configuración)

Nota:
- La tabla de contenidos (campo TOC) puede requerir actualización manual en Word al abrir.

---

## 7. Golden tests (recomendación)

Para pruebas de integración automáticas:

- Verificar que el paquete OpenXML contiene:
  - `/word/styles.xml`
  - `/word/numbering.xml`
  - `/word/settings.xml`
  - `/word/document.xml`
  - `/word/header*.xml` y `/word/footer*.xml`
  - `/word/media/*` (logo)

- Verificar relaciones correctas en:
  - `/word/_rels/document.xml.rels`

- Opcional:
  - Calcular hash del zip del `.dotx` generado bajo condiciones deterministas (misma plataforma y build)

---

## 8. Estado del documento

Este documento habilita la implementación de la primera PoC funcional y la batería de pruebas de integración. El siguiente paso recomendado es implementar:

1. Loader + SchemaValidator
2. SemanticValidator mínimo
3. Writers: StylesPartWriter + NumberingPartWriter + MainDocumentWriter
4. Generación de `template.dotx` y `sample.docx`

