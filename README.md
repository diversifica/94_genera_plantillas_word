# TemplateGen – Sistema Generador de Plantillas Word (OpenXML)

Generación determinista de documentos Word (.docx) a partir de perfiles JSON y contenido estructurado.

## Características

- ✅ **Configuración de Página**: Márgenes, orientación, tamaño de página
- ✅ **Estilos Personalizados**: Párrafos, encabezados, listas
- ✅ **Numeración Multinivel**: Listas numeradas, viñetas, encabezados numerados
- ✅ **Generación de Contenido**: Párrafos, encabezados, listas, tablas, imágenes
- ✅ **Validación JSON Schema**: Validación automática de perfiles
- ✅ **Logging Estructurado**: Trazabilidad completa de operaciones

## Instalación

```bash
git clone https://github.com/diversifica/94_genera_plantillas_word.git
cd 94_genera_plantillas_word
dotnet build
```

## Uso Básico

### Generar documento desde perfil

```bash
dotnet run --project src/TemplateGen.Cli -- \
  --profile profiles/acme_consulting/profile.json \
  --output output
```

### Generar documento con contenido

```bash
dotnet run --project src/TemplateGen.Cli -- \
  --profile profiles/acme_consulting/profile.json \
  --content profiles/acme_consulting/sample_content_complex.json \
  --output output_phase7
```

## Estructura de Archivos

### Perfil de Plantilla (`profile.json`)

Define la estructura del documento:

```json
{
  "template_id": "acme_001",
  "version": "1.0",
  "document": {
    "page_size": "A4",
    "orientation": "portrait",
    "margins": { "top": 1440, "bottom": 1440, "left": 1440, "right": 1440 }
  },
  "styles": {
    "paragraph_styles": [...],
    "character_styles": [...]
  },
  "numbering": {
    "heading_numbering": {...},
    "lists": [...]
  }
}
```

### Archivo de Contenido (`content.json`)

Define el contenido a generar:

```json
{
  "document_id": "doc_001",
  "language": "es-ES",
  "sections": [
    {
      "section_id": "main",
      "content": [
        { "type": "heading", "level": 1, "text": "Título Principal" },
        { "type": "paragraph", "style_id": "Normal", "text": "Contenido..." },
        {
          "type": "table",
          "rows": [
            {
              "cells": [
                { "content": [{ "type": "paragraph", "style_id": "Normal", "text": "Celda 1" }] }
              ]
            }
          ]
        },
        {
          "type": "image",
          "source": "path/to/image.png",
          "width": 200,
          "height": 100,
          "alt_text": "Descripción"
        }
      ]
    }
  ]
}
```

## Tipos de Contenido Soportados

| Tipo | Descripción | Propiedades |
|------|-------------|-------------|
| `paragraph` | Párrafo de texto | `style_id`, `text` |
| `heading` | Encabezado | `level` (1-9), `text` |
| `list` | Lista numerada/viñetas | `list_type` ("numbered"/"bullet"), `items` |
| `table` | Tabla con filas y celdas | `rows` (array de `cells`) |
| `image` | Imagen inline | `source`, `width`, `height`, `alt_text` |

## Documentación Completa

Ver [docs/00_indice_documentacion_templategen.md](docs/00_indice_documentacion_templategen.md) para documentación detallada.

## Contribución

Lee [Contributing.md](Contributing.md) antes de realizar cambios. Este proyecto sigue una metodología estricta de diseño antes de código.

## Licencia

MIT License - Ver [LICENSE](LICENSE)
