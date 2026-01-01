# Arquitectura de Implementación del Motor (OpenXML)

## Sistema Generador de Plantillas Word

---

## 1. Propósito del documento

Este documento define la **arquitectura de implementación** del motor generador de plantillas Word basado en OpenXML. Su objetivo es especificar:

- Estructura del proyecto y módulos
- Responsabilidades por capa
- Flujo de generación end-to-end
- Estrategia de validación (schema + semántica)
- Diseño de writers OpenXML y orden de escritura
- Salidas, reportes y logging

Este documento está alineado con:
- Requisitos del Sistema
- Esquema del Perfil de Cliente
- Modelo de Dominio
- JSON Schema del Perfil

---

## 2. Stack tecnológico (decisión de implementación)

Recomendación:
- Lenguaje: C#
- Plataforma: .NET (LTS)
- Librería: DocumentFormat.OpenXml (OpenXML SDK)

Restricciones:
- No requiere Microsoft Word instalado
- Generación de .dotx y .docx mediante OpenXML puro

---

## 3. Estructura de repositorio

Estructura recomendada:

- /docs
  - 01_requisitos.md
  - 02_esquema_perfil_cliente.md
  - 03_modelo_dominio.md
  - 04_json_schema_perfil.md
  - 05_arquitectura_implementacion.md
- /schemas
  - template-profile.schema.json
- /profiles
  - /<client_id>
    - profile.json
    - /assets
      - logo.png
      - ...
    - /output
      - template.dotx
      - sample.docx
      - report.json
      - report.md
    - changelog.md
- /src
  - TemplateGen.Cli
  - TemplateGen.Core
  - TemplateGen.Domain
  - TemplateGen.OpenXml
  - TemplateGen.Validation
  - TemplateGen.Reporting
- /tests
  - TemplateGen.UnitTests
  - TemplateGen.IntegrationTests

---

## 4. Capas y responsabilidades

### 4.1 TemplateGen.Cli

Responsable de:
- Punto de entrada
- Parsing de argumentos
- Selección de perfil
- Control de ejecución y códigos de salida

No contiene lógica de negocio.

---

### 4.2 TemplateGen.Core

Responsable de:
- Orquestación del pipeline
- Coordinación entre validación, composición y render

Expone el servicio:
- TemplateGenerationService

---

### 4.3 TemplateGen.Domain

Responsable de:
- Entidades y objetos de valor
- Invariantes
- Servicios de dominio (resolución de estilos, binder de numeración, assembler de secciones)

No referencia OpenXML.

---

### 4.4 TemplateGen.Validation

Responsable de:
- Validación contra JSON Schema
- Validación semántica
- Normalización y defaults documentados

Expone:
- SchemaValidator
- SemanticValidator

---

### 4.5 TemplateGen.OpenXml

Responsable de:
- Renderizado OpenXML de TemplateModel
- Writers especializados por parte

No contiene reglas de dominio.

---

### 4.6 TemplateGen.Reporting

Responsable de:
- Generación de informes
- Serialización de reportes (JSON y Markdown)
- Registro de métricas de generación

---

## 5. Pipeline de ejecución (end-to-end)

Flujo estricto:

1. LoadProfile
2. ValidateSchema (JSON Schema)
3. ValidateSemantics (invariantes + referencias)
4. ComposeTemplateModel (dominio)
5. RenderTemplate (OpenXML)
6. GenerateSampleDocument (OpenXML)
7. EmitArtifacts (output + report)

No se permite render si falla alguna validación.

---

## 6. Diseño del motor: TemplateGenerationService

Interfaz recomendada:

- Generate(profilePath, outputDir, options)

Opciones:
- generateDotx (bool)
- generateSampleDocx (bool)
- strictMode (bool)
- failOnWarnings (bool)

Salida:
- GenerationResult (éxito/fracaso, rutas, warnings, métricas)

---

## 7. Validación

### 7.1 Validación por schema

- Validación estricta contra `template-profile.schema.json`
- Errores deben incluir:
  - code
  - message
  - json_path
  - severity

### 7.2 Validación semántica

Reglas mínimas:

- `Normal` existe en estilos de párrafo
- Estilos obligatorios presentes
- `based_on` resoluble y sin ciclos
- `style_bindings` completos para niveles de títulos
- `section_id` referenciados existen
- Assets referenciados existen

Salida:
- Lista de errores y warnings estructurados

---

## 8. Composición del TemplateModel

Objetivo:
- Resolver herencias
- Materializar propiedades efectivas
- Preparar estructuras listas para render

Servicios involucrados:
- StyleResolver
- NumberingBinder
- SectionAssembler
- AssetResolver
- TocPreparer

Salida:
- TemplateModel inmutable

---

## 9. Render OpenXML (.dotx / .docx)

### 9.1 Estrategia de packaging

- Para .docx: WordprocessingDocumentType.Document
- Para .dotx: WordprocessingDocumentType.Template

El motor debe construir el paquete con:
- MainDocumentPart
- StyleDefinitionsPart
- NumberingDefinitionsPart
- DocumentSettingsPart
- HeaderParts / FooterParts
- Media parts (imágenes)

---

## 10. Writers OpenXML

### 10.1 Principio

Cada writer:
- Recibe TemplateModel
- Escribe en un OpenXML Part
- No modifica TemplateModel
- No decide reglas de dominio

---

### 10.2 Writers obligatorios

- PackageWriter
  - Crea documento, partes y relaciones

- StylesPartWriter
  - Escribe estilos (paragraph + character)
  - Define Normal y títulos

- NumberingPartWriter
  - Escribe numbering para títulos (multinivel)
  - Escribe numbering para listas

- SettingsPartWriter
  - Escribe compatibilidad, idioma, tabs, etc.

- MainDocumentWriter
  - Crea estructura vacía con secciones
  - Inserta saltos de sección necesarios
  - Inserta placeholders (TOC)

- HeaderWriter
  - Genera cabeceras según plan
  - Inserta logo si aplica

- FooterWriter
  - Genera pies según plan
  - Inserta numeración

- MediaWriter
  - Inserta imágenes en partes
  - Crea relaciones

- RelationshipsWriter
  - Verifica relaciones internas
  - (Opcional) validación final de integridad

---

## 11. Orden de escritura recomendado

Para evitar inconsistencias:

1. Crear paquete y MainDocumentPart
2. Crear y escribir StyleDefinitionsPart
3. Crear y escribir NumberingDefinitionsPart
4. Crear y escribir DocumentSettingsPart
5. Crear y escribir MediaParts (si existen)
6. Crear y escribir Header/Footer Parts
7. Escribir MainDocument body:
   - secciones
   - encabezados/pies asociados
   - placeholder de TOC
8. Guardar y cerrar

---

## 12. Documento de prueba (sample.docx)

El sistema debe poder generar un documento de prueba para validación visual, incluyendo:

- Portada
- Títulos de ejemplo (1, 1.1, 1.1.1)
- Párrafos Normal
- Lista numerada y con viñetas
- Bloque de nota
- Bloque de cita
- Código de bloque e inline
- Pie de figura con una imagen (si hay assets)
- Tabla de contenidos (campo TOC)

Nota:
- La tabla de contenidos puede requerir actualización al abrir en Word.

---

## 13. Reportes y logging

### 13.1 Reporte JSON

Debe incluir:
- metadata del perfil
- versión del motor
- hashes de entradas
- lista de validaciones realizadas
- warnings y errores
- rutas de salida

### 13.2 Reporte Markdown

Debe incluir:
- resumen humano
- checklist de estilos generados
- esquema de numeración
- secciones creadas
- assets insertados

---

## 14. Pruebas

### 14.1 Unit tests

Cobertura mínima:
- resoluciones de herencia
- validación semántica
- binding de numeración

### 14.2 Integration tests

- Generar .dotx y .docx con un perfil ejemplo
- Verificar que el paquete contiene las partes esperadas
- Validar que no hay referencias rotas

---

## 15. Compatibilidad y robustez

- El motor debe soportar perfiles con extensiones desconocidas ignorándolas
- Los cambios en schema_version deben gestionarse mediante un CompatibilityMatrix
- Deben existir mensajes de error accionables

---

## 16. Criterios de aceptación

La arquitectura se considera implementable cuando:

- Existe una estructura de proyecto clara con separación por capas
- El pipeline está definido y es determinista
- Los writers están aislados y tienen responsabilidades únicas
- La salida incluye plantilla y documento de prueba con reportes

---

## 17. Estado del documento

Este documento habilita la siguiente fase: implementación del esqueleto de solución, definición de un perfil de ejemplo completo y creación de la primera generación funcional (PoC) con estilos y numeración multinivel.
