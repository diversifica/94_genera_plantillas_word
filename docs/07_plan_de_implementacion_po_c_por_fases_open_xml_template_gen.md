# Plan de Implementación PoC por Fases

## Sistema Generador de Plantillas Word (OpenXML)

---

## 1. Propósito del documento

Este documento define un **plan de implementación incremental** (PoC) para construir el motor generador de plantillas Word basado en OpenXML, minimizando riesgo y asegurando resultados verificables en cada fase.

Principios:
- Entregables pequeños y comprobables
- Criterios de aceptación objetivos
- Evitar "big bang" de implementación
- Aislar complejidad (numeración, secciones, headers/footers) por etapas

---

## 2. Definición de PoC

Se considera PoC completada cuando el sistema puede:

- Validar un `profile.json` contra el JSON Schema
- Ejecutar validación semántica mínima
- Generar `template.dotx`
- Generar `sample.docx` con contenido de prueba
- Producir `report.json` y `report.md`

Usando el perfil golden sample:
- `profiles/acme_consulting/profile.json`

---

## 3. Fases del plan

### Fase 0 — Preparación del repositorio

Objetivo:
- Dejar el repositorio listo para desarrollo y pruebas.

Tareas:
- Crear estructura `/src`, `/tests`, `/schemas`, `/profiles`, `/docs`
- Añadir `template-profile.schema.json` a `/schemas`
- Añadir golden sample a `/profiles/acme_consulting`
- Definir convención de build (Debug/Release)

Entregables:
- Repositorio con estructura completa
- Perfil golden sample y assets

Criterios de aceptación:
- El repo compila (aunque no genere aún)
- El perfil y schema están presentes en rutas estándar

---

### Fase 1 — CLI mínima y carga de perfil

Objetivo:
- Poder invocar el sistema por consola y cargar el perfil.

Tareas:
- Crear `TemplateGen.Cli`
- Implementar parsing de argumentos:
  - `--profile <path>`
  - `--output <dir>`
  - `--strict`
- Implementar `ProfileLoader`:
  - Leer JSON UTF-8
  - Deserializar a DTOs

Entregables:
- Ejecutable CLI que imprime metadata del perfil

Criterios de aceptación:
- `TemplateGen.Cli --profile profiles/acme_consulting/profile.json --output profiles/acme_consulting/output` carga y muestra `client_id`, `profile_version`
- Manejo de errores: archivo inexistente / JSON inválido

---

### Fase 2 — Validación JSON Schema

Objetivo:
- Bloquear ejecución si el perfil no cumple el schema.

Tareas:
- Integrar validador JSON Schema (draft 2020-12)
- Cargar schema desde `/schemas/template-profile.schema.json`
- Devolver lista de errores con:
  - severity
  - json_path
  - message

Entregables:
- `SchemaValidator` funcional
- `report.json` básico con resultados de validación

Criterios de aceptación:
- El golden sample valida sin errores
- Si se elimina un campo obligatorio, la ejecución falla con error claro

---

### Fase 3 — Validación semántica mínima

Objetivo:
- Verificar invariantes esenciales antes de render.

Reglas mínimas:
- Existe estilo `Normal`
- Existen `Title1`, `Title2`, `Title3`
- `based_on` resoluble
- `heading_numbering.style_bindings` referencian estilos existentes
- `sections.items` no vacía y `section_id` únicos
- `headers_footers.items.section_id` existen
- `assets.items` con rutas relativas válidas y archivos existentes

Entregables:
- `SemanticValidator` mínimo
- `report.json` y `report.md` con errores/warnings

Criterios de aceptación:
- El golden sample pasa validación semántica
- Si se referencia un `logo_asset_id` inexistente, la ejecución falla con error estructurado

---

### Fase 4 — Render mínimo de plantilla (.dotx) con estilos

Objetivo:
- Generar un `.dotx` válido con estilos definidos.

Tareas:
- Crear `TemplateGen.OpenXml`
- Implementar `OpenXmlTemplateRenderer` (sólo plantilla)
- Implementar `StylesPartWriter`:
  - Generar `styles.xml`
  - Crear estilos de párrafo y carácter
  - Marcar `Normal` como estilo por defecto

Entregables:
- `template.dotx` generado

Criterios de aceptación:
- Word abre `template.dotx` sin advertencias
- En Word aparecen estilos: Normal, Título 1/2/3, listas, etc.

---

### Fase 5 — Numeración multinivel de títulos

Objetivo:
- Implementar `numbering.xml` y enlazarlo con estilos de título.

Tareas:
- Implementar `NumberingPartWriter`:
  - `abstractNum` multinivel
  - `num` y asociaciones
- Implementar `NumberingBinder` (dominio) si no existe

Entregables:
- `template.dotx` con numeración de títulos operativa

Criterios de aceptación:
- En un documento creado desde la plantilla:
  - Aplicar Título 1 produce "1." automáticamente
  - Título 2 produce "1.1."
  - Título 3 produce "1.1.1."

---

### Fase 6 — Secciones + headers/footers + numeración de páginas

Objetivo:
- Materializar la estructura por secciones y sus encabezados/pies.

Tareas:
- Implementar `MainDocumentWriter`:
  - Crear `document.xml` con secciones según `sections.items`
  - Insertar section breaks mínimos
- Implementar `HeaderWriter` y `FooterWriter`:
  - Crear parts por sección
  - Insertar campos de numeración de página
  - Insertar textos left/center/right
- Implementar page numbering (roman/arabic) por sección

Entregables:
- `template.dotx` con:
  - Portada sin numeración
  - TOC con romanos
  - Body con arábigos reiniciando en 1

Criterios de aceptación:
- Word muestra numeración por sección según golden sample
- Cabecera muestra textos configurados

---

### Fase 7 — Inserción de assets (logo)

Objetivo:
- Insertar imágenes en cabecera/pie con relaciones correctas.

Tareas:
- Implementar `MediaWriter`:
  - Añadir `ImagePart`
  - Registrar relación en el part correspondiente
  - Insertar `Drawing` en el header
- Validar dimensiones básicas y fallback

Entregables:
- `template.dotx` con logo en cabecera (cuando aplique)

Criterios de aceptación:
- El logo aparece en Word en las secciones configuradas
- No hay referencias rotas en `.rels`

---

### Fase 8 — Generación de `sample.docx`

Objetivo:
- Crear un documento de prueba que use la plantilla y demuestre estilos.

Tareas:
- Implementar `SampleDocumentGenerator`:
  - Crear doc con contenido mínimo
  - Aplicar estilos (títulos, listas, cita, nota, código)
  - Insertar campo TOC

Entregables:
- `sample.docx`

Criterios de aceptación:
- Al abrir `sample.docx`:
  - Se ven títulos numerados
  - Secciones y numeración de página correctas
  - Listas coherentes
  - Estilos visibles

---

### Fase 9 — Reportes finales y golden tests

Objetivo:
- Cerrar PoC con trazabilidad y pruebas automatizadas.

Tareas:
- Completar `report.json` con métricas y hashes
- Generar `report.md` humano
- Integration tests:
  - Verificar existencia de parts (`styles.xml`, `numbering.xml`, etc.)
  - Verificar relaciones (document.xml.rels)
  - Verificar presencia de media

Entregables:
- PoC completa con pipeline robusto

Criterios de aceptación:
- Tests pasan en CI local
- Generación determinista sobre el mismo entorno

---

## 4. Estrategia de control de cambios

- Cada fase se integra en una rama/PR
- Cada PR incluye:
  - Cambio mínimo
  - Tests asociados
  - Actualización del changelog

---

## 5. Riesgos y mitigaciones

- Numeración multinivel: abordar en fase separada (Fase 5)
- Secciones y paginación: fase dedicada (Fase 6)
- Imágenes y relaciones: fase dedicada (Fase 7)

---

## 6. Definición de "Done" por fase

Una fase se considera finalizada cuando:
- Pasa criterios de aceptación
- Tiene tests asociados (si aplica)
- No introduce warnings severos

---

## 7. Estado del documento

Este plan guía la implementación PoC. Tras completar la PoC, el siguiente paso recomendado es:

- Endurecer la validación semántica
- Añadir perfiles reales de clientes
- Implementar estrategias de migración de versiones de perfil

