# Modelo de Dominio del Motor Generador (OpenXML)

## Sistema Generador de Plantillas Word

---

## 1. Propósito del documento

Este documento define el **modelo de dominio interno** del motor generador de plantillas Word basado en OpenXML. Su objetivo es establecer:

- Las entidades y objetos de valor que componen el sistema
- Los límites de responsabilidad (agregados) y sus invariantes
- Los servicios de dominio necesarios para construir plantillas
- El flujo de construcción (pipeline) del documento Word

Este modelo conecta el **Perfil de Cliente** (entrada declarativa) con la **implementación** del motor, reduciendo ambigüedades y evitando rediseños posteriores.

---

## 2. Principios del modelo de dominio

1. **Entrada declarativa, ejecución determinista**
2. **Idempotencia**: regenerar produce el mismo resultado
3. **Separación estricta**:
   - Dominio (qué construir)
   - Infraestructura OpenXML (cómo escribirlo)
4. **Invariantes explícitas**: el dominio valida coherencia antes de emitir OpenXML
5. **Construcción por fases**: el documento se ensambla en un orden controlado
6. **Extensión compatible**: nuevos módulos deben añadirse sin alterar invariantes existentes

---

## 3. Contextos y límites de responsabilidad

El motor se divide conceptualmente en los siguientes contextos:

- **Profile Ingestion**: carga, parseo y validación del perfil
- **Template Domain**: representación interna del documento a construir
- **OpenXML Rendering**: conversión a artefactos OpenXML
- **Artifacts Output**: generación de salidas y reportes

La lógica del dominio no debe depender de OpenXML. OpenXML es un detalle de infraestructura.

---

## 4. Entidades y objetos de valor

### 4.1 Entidad: TemplateProfile

Representa un perfil de cliente ya validado.

Atributos:
- client_id
- profile_version
- metadata
- document_settings
- font_catalog
- style_catalog
- numbering_catalog
- section_plan
- header_footer_plan
- toc_plan
- asset_catalog
- ruleset

Invariante:
- Un TemplateProfile debe estar validado en integridad sintáctica y semántica antes de ser consumido por el generador.

---

### 4.2 Objeto de valor: DocumentSettings

Define propiedades globales del documento.

Atributos:
- page_size
- orientation
- margins
- language
- default_tab_stop

Invariantes:
- Márgenes positivos y dentro de límites razonables
- page_size y orientation compatibles

---

### 4.3 Objeto de valor: FontCatalog

Catálogo de fuentes con roles definidos.

Atributos:
- body_font
- headings_font
- monospace_font

Cada fuente define:
- font_name
- fallback_fonts

Invariante:
- Todas las fuentes referenciadas por estilos deben existir en el catálogo o referenciarse explícitamente.

---

### 4.4 Entidad: StyleCatalog

Colección de estilos del documento.

Subcolecciones:
- paragraph_styles
- character_styles

Invariantes:
- Identificadores únicos
- No hay ciclos de herencia
- Existencia del estilo base (Normal)
- Estilos obligatorios presentes

---

### 4.5 Objeto de valor: ParagraphStyleDefinition

Define un estilo de párrafo.

Atributos:
- style_id
- style_name
- based_on
- run_properties (fuente, tamaño, negrita, cursiva, color)
- paragraph_properties (alineación, sangría, espaciados, interlineado)
- pagination_properties (keep_with_next, page_break_before)

Invariantes:
- based_on debe existir o ser nulo
- Valores de espaciado y sangría dentro de límites

---

### 4.6 Objeto de valor: CharacterStyleDefinition

Define un estilo de carácter.

Atributos:
- style_id
- style_name
- run_properties

Invariante:
- No define propiedades de párrafo

---

### 4.7 Entidad: NumberingCatalog

Gestiona esquemas de numeración.

Subcolecciones:
- heading_numbering_scheme
- list_numbering_schemes

Invariantes:
- Esquemas con identificadores únicos
- Los niveles asociados a títulos deben mapearse a estilos existentes

---

### 4.8 Objeto de valor: HeadingNumberingScheme

Define numeración multinivel para títulos.

Atributos:
- scheme_id
- levels (lista ordenada)
- restart_policy
- style_bindings (nivel -> style_id)

Cada nivel define:
- level_index
- number_format (decimal, roman, letter)
- level_text_pattern (ej. "%1.%2.")
- start_at
- indentation

Invariantes:
- level_text_pattern debe ser consistente con niveles definidos
- style_bindings completos para niveles soportados

---

### 4.9 Entidad: SectionPlan

Planifica la estructura por secciones.

Atributos:
- sections (lista ordenada)

Cada sección define:
- section_id
- type (cover, toc, body, annexes, custom)
- page_numbering (none, roman, arabic, custom)
- start_page_behavior
- page_layout_overrides (opcional)

Invariantes:
- Orden lógico válido (cover antes de body, etc.)
- Cada section_id es único

---

### 4.10 Entidad: HeaderFooterPlan

Define encabezados y pies por sección.

Atributos:
- items (lista de definiciones)

Cada definición:
- section_id
- header_definition
- footer_definition

Invariantes:
- section_id debe existir en SectionPlan

---

### 4.11 Objeto de valor: HeaderFooterDefinition

Define contenido de cabecera o pie.

Atributos:
- text_left
- text_center
- text_right
- include_page_number
- page_number_format
- show_on_first_page
- include_logo (referencia a Asset)

Invariantes:
- Si include_logo es true, el asset debe existir

---

### 4.12 Entidad: TocPlan

Configura índices automáticos.

Atributos:
- enabled
- levels
- include_annexes
- style_mapping
- toc_title (opcional)

Invariantes:
- Si enabled es true, levels debe ser >= 1

---

### 4.13 Entidad: AssetCatalog

Gestiona recursos externos.

Atributos:
- assets (lista)

Cada asset:
- asset_id
- relative_path
- type (image, logo, other)
- usage (header, cover, watermark, etc.)

Invariantes:
- asset_id único
- relative_path válido

---

### 4.14 Entidad: RuleSet

Conjunto de reglas declarativas asociadas al perfil.

Atributos:
- forbid_manual_formatting
- auto_map_bold_to_strong_emphasis
- enforce_styles_only
- otros flags extensibles

Nota:
- Estas reglas afectan al comportamiento del motor al generar el documento de prueba y a reportes.

---

## 5. Agregados y responsabilidades

### 5.1 Aggregate Root: TemplateProfile

Responsable de:
- Mantener coherencia global del perfil
- Exponer vistas consistentes al generador

Invariantes del agregado:
- Catálogo de estilos completo y coherente
- Esquemas de numeración válidos y mapeados
- Secciones definidas y referenciables por headers/footers

---

## 6. Servicios de dominio

### 6.1 ProfileValidator

Responsabilidad:
- Validación sintáctica (schema)
- Validación semántica (invariantes)

Salida:
- Resultado de validación con lista de errores estructurados

---

### 6.2 TemplateComposer

Responsabilidad:
- Construir un modelo interno de documento listo para renderizar
- Resolver herencias y defaults documentados
- Preparar mapeos estilo <-> numeración

Salida:
- TemplateModel (modelo interno final)

---

### 6.3 StyleResolver

Responsabilidad:
- Resolver herencias de estilos
- Detectar ciclos
- Materializar propiedades efectivas

---

### 6.4 NumberingBinder

Responsabilidad:
- Vincular esquemas de numeración con estilos de títulos y listas
- Validar patrones de nivel

---

### 6.5 SectionAssembler

Responsabilidad:
- Materializar secciones del documento
- Aplicar configuraciones de paginación y layout por sección
- Coordinar headers/footers

---

## 7. Modelo de salida interno: TemplateModel

El TemplateModel es la representación interna lista para renderizar.

Componentes:
- DocumentSettings (efectivos)
- Styles (efectivos)
- Numbering (efectivo)
- Sections (efectivas)
- Headers/Footers (materializados)
- TOC placeholders (si aplica)
- Assets resolved (rutas validadas, tipos)

Invariante:
- TemplateModel debe ser completo, sin referencias pendientes.

---

## 8. Infraestructura: OpenXML Rendering

### 8.1 Renderer: OpenXmlTemplateRenderer

Responsabilidad:
- Convertir TemplateModel a OpenXML

Restricciones:
- No debe aplicar reglas de negocio
- No debe inferir propiedades
- Debe limitarse a escribir la estructura OpenXML correcta

### 8.2 Writers especializados

Se definen escritores por componente:
- StylesPartWriter
- NumberingPartWriter
- MainDocumentWriter
- HeaderWriter
- FooterWriter
- SettingsWriter
- RelationshipsWriter
- MediaWriter

Cada writer opera sobre una porción acotada del paquete OpenXML.

---

## 9. Pipeline de construcción

El sistema deberá seguir este orden estricto:

1. LoadProfile
2. ValidateSchema
3. ValidateSemantics
4. ComposeTemplateModel
   - ResolveStyles
   - BindNumbering
   - AssembleSections
   - ResolveAssets
   - PrepareTOC
5. RenderOpenXML
6. EmitArtifacts

Invariante:
- No se renderiza si no se supera validación.

---

## 10. Gestión de errores

Los errores deben ser:
- Estructurados
- Clasificados (schema, semantic, rendering, assets)
- Asociados a una ruta del perfil (ej. styles.paragraph_styles.Title1)

Se debe generar un informe de validación en cada ejecución.

---

## 11. Compatibilidad y evolución

### 11.1 Versionado del perfil

El motor debe declarar:
- Versión mínima soportada
- Versiones conocidas

### 11.2 Estrategia de evolución

- Nuevos campos deben ser opcionales con valores por defecto documentados
- Nuevos bloques deben ser ignorables por motores antiguos (si procede)

---

## 12. Criterios de aceptación del modelo

Se considera aceptado este modelo cuando:

- Todas las piezas del Perfil de Cliente tienen representación explícita en el dominio
- Existe un pipeline determinista de composición y render
- La lógica de dominio está aislada de OpenXML
- Es posible añadir nuevos estilos o módulos sin reescribir el motor

---

## 13. Estado del documento

Este documento define el modelo de dominio inicial del motor generador y habilita la siguiente fase: definición del esquema formal (JSON Schema) y diseño de la arquitectura de implementación (proyecto, paquetes, clases y writers OpenXML).

