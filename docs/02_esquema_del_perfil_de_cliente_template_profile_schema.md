# Esquema del Perfil de Cliente

## Sistema Generador de Plantillas Word (OpenXML)

---

## 1. Propósito del documento

Este documento define el **esquema lógico y estructural del Perfil de Cliente** (Template Profile), que actúa como contrato de entrada entre la especificación declarativa y el motor generador de plantillas Word.

El perfil de cliente describe de forma completa, validable y versionable todas las decisiones necesarias para generar una plantilla Word profesional sin intervención manual.

Este esquema está diseñado para:
- Ser extensible sin romper compatibilidad
- Ser validable automáticamente
- Ser independiente del motor de generación

---

## 2. Principios del esquema

1. El perfil es **autocontenido**
2. El perfil es **determinista**
3. El perfil no contiene lógica, solo datos
4. Todo parámetro es explícito (no hay valores implícitos)
5. La ausencia de un bloque implica que el motor debe aplicar valores por defecto documentados

---

## 3. Estructura general del perfil

El perfil se compone de los siguientes bloques principales:

- metadata
- document
- fonts
- styles
- numbering
- sections
- headers_footers
- toc
- assets
- rules

Cada bloque es independiente y evolutivo.

---

## 4. Bloque metadata

### Propósito
Identificar, versionar y trazar el perfil.

### Campos obligatorios

- client_id
- client_name
- profile_version
- created_at
- updated_at
- author

### Consideraciones

- profile_version debe seguir versionado semántico
- created_at y updated_at deben estar en formato ISO 8601

---

## 5. Bloque document

### Propósito
Definir las propiedades físicas y globales del documento.

### Campos

- page_size
- orientation
- margins
- language
- default_tab_stop

### Detalle

- page_size: A4, Letter, Legal, u otros
- orientation: portrait | landscape
- margins: top, bottom, left, right (en unidades normalizadas)
- language: código de idioma (ej. es-ES)

---

## 6. Bloque fonts

### Propósito
Centralizar las tipografías utilizadas.

### Campos

- body
- headings
- monospace

### Detalle

Cada fuente debe definir:
- font_name
- fallback_fonts

---

## 7. Bloque styles

### Propósito
Definir todos los estilos del documento.

### Subbloques

- paragraph_styles
- character_styles

---

### 7.1 paragraph_styles

Cada estilo de párrafo debe definir:

- style_id
- style_name
- based_on
- font
- font_size
- line_spacing
- spacing_before
- spacing_after
- alignment
- keep_with_next
- page_break_before

Estilos obligatorios:

- Normal
- Title1
- Title2
- Title3
- BulletList
- NumberedList
- Quote
- Note
- Example
- CodeBlock
- FigureCaption

---

### 7.2 character_styles

Cada estilo de carácter debe definir:

- style_id
- style_name
- font
- font_size
- bold
- italic
- color

Estilos obligatorios:

- Emphasis
- StrongEmphasis
- Term
- InlineCode

---

## 8. Bloque numbering

### Propósito
Definir esquemas de numeración multinivel.

### Subbloques

- heading_numbering
- list_numbering

---

### 8.1 heading_numbering

Define:

- levels
- format
- restart_on_section
- associated_styles

Debe permitir:
- Numeración decimal
- Numeración alfabética para anexos

---

### 8.2 list_numbering

Define:

- numbered_lists
- bullet_lists

Cada lista debe definir:
- indent
- alignment
- restart_behavior

---

## 9. Bloque sections

### Propósito
Definir la estructura lógica del documento.

### Tipos de sección

- cover
- toc
- body
- annexes

Cada sección debe definir:

- section_id
- type
- page_numbering
- start_page

---

## 10. Bloque headers_footers

### Propósito
Configurar encabezados y pies por sección.

### Campos

- section_id
- header
- footer

Cada uno puede definir:

- text_left
- text_center
- text_right
- page_number_format
- show_on_first_page

---

## 11. Bloque toc

### Propósito
Configurar índices automáticos.

### Campos

- enabled
- levels
- include_annexes
- style_mapping

---

## 12. Bloque assets

### Propósito
Gestionar recursos externos.

### Campos

- logos
- images

Cada recurso debe definir:

- asset_id
- relative_path
- usage

---

## 13. Bloque rules

### Propósito
Definir reglas de saneamiento y uso.

### Ejemplos de reglas

- forbid_manual_formatting
- auto_map_bold_to_strong_emphasis
- enforce_styles_only

---

## 14. Extensibilidad

El esquema deberá permitir:

- Nuevos bloques
- Nuevos estilos
- Nuevas reglas

Sin invalidar perfiles existentes.

---

## 15. Validación

El perfil deberá validarse antes de su uso mediante:

- Esquema JSON/YAML
- Reglas semánticas adicionales

Una validación fallida debe impedir la generación.

---

## 16. Relación con el motor

El motor OpenXML:

- No infiere valores
- No corrige errores
- Solo ejecuta perfiles válidos

Toda lógica reside en el diseño del perfil.

---

## 17. Estado del documento

Este documento define la versión inicial del esquema lógico del Perfil de Cliente y sirve como base para el diseño del esquema formal (JSON Schema) y del modelo de dominio interno.

