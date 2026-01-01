# Checklist Diario de Implementación

## TemplateGen (OpenXML)

---

## 1. Propósito

Este checklist define los controles diarios recomendados para desarrollar el motor TemplateGen de forma **determinista, mantenible y sin regresiones**.

Uso previsto:
- Antes de empezar una tarea
- Antes de abrir PR
- Antes de merge

---

## 2. Checklist antes de empezar una tarea

### 2.1 Contexto y alcance

- He identificado la fase (según el Plan PoC) en la que estoy trabajando.
- El cambio es incremental (no mezcla 2 fases en una sola tarea).
- He definido el criterio de aceptación de hoy (resultado verificable).

### 2.2 Documentación alineada

- Los cambios respetan:
  - Requisitos
  - Esquema del Perfil
  - Modelo de Dominio
  - JSON Schema
  - Convenciones OpenXML

Si hay desviación:
- Está registrada y justificada en documentación.

---

## 3. Checklist durante la implementación

### 3.1 Dominio vs Infraestructura

- La lógica de decisiones (defaults, herencias, reglas) está en dominio/composición.
- Los writers OpenXML no aplican reglas de negocio.
- El TemplateModel es inmutable durante el render.

### 3.2 Determinismo

- Orden estable de:
  - estilos
  - numeración
  - secciones
  - assets
- No se introducen timestamps ni GUIDs aleatorios.
- La salida es reproducible con el mismo input y entorno.

### 3.3 Errores estructurados

- Errores y warnings incluyen:
  - código
  - mensaje
  - severidad
  - ruta (`json_path`) cuando aplique
- No se lanzan excepciones genéricas.

---

## 4. Checklist de validación

### 4.1 Validación de entrada

- El perfil valida contra JSON Schema.
- La validación semántica bloquea render si hay errores.
- Las extensiones desconocidas se ignoran sin romper ejecución (si procede).

### 4.2 Validación OpenXML

- Se generan partes esperadas:
  - `word/styles.xml`
  - `word/numbering.xml`
  - `word/settings.xml`
  - `word/document.xml`
  - headers/footers si aplica
  - media si aplica
- No hay relaciones rotas (`document.xml.rels`).

---

## 5. Checklist de ejecución local

### 5.1 Comandos y artefactos

- He ejecutado el CLI con el golden sample:
  - `--profile profiles/acme_consulting/profile.json`
  - `--output profiles/acme_consulting/output`

- Se han generado:
  - `template.dotx` (si aplica)
  - `sample.docx` (si aplica)
  - `report.json`
  - `report.md`

### 5.2 Inspección rápida

- Word abre `template.dotx` sin advertencias.
- Word abre `sample.docx` sin advertencias.
- Verificación visual rápida:
  - Estilos visibles
  - Numeración de títulos (si aplica)
  - Secciones y numeración de páginas (si aplica)
  - Logo en cabecera (si aplica)

---

## 6. Checklist de pruebas

### 6.1 Unit tests

- Se han añadido o actualizado tests cuando el cambio afecta:
  - herencia de estilos
  - binding de numeración
  - validaciones semánticas

### 6.2 Integration tests

- Si el cambio afecta OpenXML:
  - existe un test que verifica parts y relaciones

- Los tests pasan en local.

---

## 7. Checklist antes de abrir PR

### 7.1 Calidad de cambios

- El PR tiene un único propósito claro.
- El commit history es limpio.
- No hay cambios no relacionados.

### 7.2 Documentación

- Si se introdujo un nuevo campo en el perfil:
  - Se actualizó JSON Schema
  - Se actualizó documentación del esquema
  - Se actualizó validación semántica

- Si se cambió un writer:
  - Se actualizó el documento de Convenciones si era necesario.

---

## 8. Checklist antes de merge

- La PoC sigue siendo ejecutable de extremo a extremo (si ya aplica).
- El golden sample sigue pasando validación y generación.
- No se han introducido antipatrones:
  - inferencias
  - secciones innecesarias
  - formato directo en sample
  - lógica de dominio dentro de writers

---

## 9. Checklist de cierre diario

- He actualizado el changelog del cliente si se generaron artefactos.
- He registrado cualquier decisión nueva o desviación en `/docs`.
- He dejado una nota clara del siguiente paso.

---

## 10. Estado del documento

Este checklist se revisará cuando:
- Se complete la PoC
- Se introduzcan perfiles reales de clientes
- Se incorporen nuevas capacidades (ej. índices de figuras, watermarks, temas)
