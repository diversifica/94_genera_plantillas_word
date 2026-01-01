# Architecture Decision Records (ADR)

## Proyecto: TemplateGen – Sistema Generador de Plantillas Word (OpenXML)

---

## Propósito del documento

Este documento registra las **decisiones arquitectónicas clave** del proyecto TemplateGen.

Su objetivo es:

- Dejar constancia explícita de *por qué* el sistema es como es
- Evitar debates recurrentes sobre decisiones ya tomadas
- Proteger el diseño frente a refactors bienintencionados pero destructivos
- Facilitar el onboarding técnico sin pérdida de contexto

Este documento **no describe cómo usar el sistema ni cómo contribuir**. Describe **criterio arquitectónico**.

Ubicación oficial:

- `/docs/ARCHITECTURE_DECISIONS.md`

---

## Convenciones

- Cada decisión tiene un identificador estable: `ADR-XXX`
- Las decisiones solo se modifican añadiendo nuevas entradas (no reescribiendo el pasado)
- Si una decisión se revisa, se añade un ADR nuevo que lo indique explícitamente

---

## ADR-001 — Uso de OpenXML SDK sin Microsoft Word

### Estado

Aceptado

### Decisión

El sistema utiliza exclusivamente **OpenXML SDK** para generar documentos `.dotx` y `.docx`, sin depender de Microsoft Word ni de automatización COM.

### Justificación

- Permite ejecución en entornos sin Office instalado
- Evita dependencias frágiles y no deterministas
- Facilita automatización, CI y despliegues futuros

### Alternativas consideradas

- Word Interop / COM automation
- Generación manual de XML sin SDK

Ambas fueron descartadas por fragilidad, dependencias externas o mayor complejidad.

### Consecuencias

- Mayor complejidad inicial
- Control total del documento generado

---

## ADR-002 — Separación estricta Dominio / Render OpenXML

### Estado

Aceptado

### Decisión

La lógica de negocio (estilos, numeración, reglas, validaciones) vive **fuera** de OpenXML.

Los writers OpenXML:
- No toman decisiones
- No contienen reglas de negocio
- Solo traducen un `TemplateModel` ya resuelto

### Justificación

- Reduce errores difíciles de depurar
- Permite tests deterministas del dominio
- Evita acoplamiento con OpenXML

### Alternativas consideradas

- Lógica mezclada en writers

Descartada por alto coste de mantenimiento.

---

## ADR-003 — Determinismo como requisito no negociable

### Estado

Aceptado

### Decisión

El sistema debe ser **determinista**:

- Mismo perfil + mismo motor = mismo resultado
- No se introducen timestamps internos
- No se usan GUIDs aleatorios sin control

### Justificación

- Permite golden tests
- Facilita debugging
- Evita salidas inconsistentes

### Consecuencias

- Algunas facilidades automáticas quedan prohibidas

---

## ADR-004 — Doble validación del perfil (Schema + Semántica)

### Estado

Aceptado

### Decisión

El perfil de cliente se valida en dos fases:

1. JSON Schema (estructura y tipos)
2. Validación semántica (referencias, invariantes, coherencia)

### Justificación

- El schema no puede capturar todas las reglas
- Permite mensajes de error más claros
- Evita renderizar configuraciones inválidas

---

## ADR-005 — Prohibición de heurísticas en el motor

### Estado

Aceptado

### Decisión

El motor **no infiere ni corrige** configuraciones incompletas:

- No inventa valores
- No asume defaults no documentados
- No "arregla" perfiles incorrectos

### Justificación

- Evita comportamientos inesperados
- Obliga a perfiles explícitos y auditables
- Mejora trazabilidad

---

## ADR-006 — Numeración definida exclusivamente en `numbering.xml`

### Estado

Aceptado

### Decisión

Toda numeración (títulos, listas) se define únicamente en `numbering.xml`.

Los estilos:
- No contienen numeración embebida

### Justificación

- Cumple el modelo OpenXML
- Evita errores de sincronización
- Facilita control multinivel

---

## ADR-007 — Tabla de contenidos como campo dinámico

### Estado

Aceptado

### Decisión

La tabla de contenidos se inserta como **campo TOC**, no como índice estático.

El motor no actualiza el TOC.

### Justificación

- Word gestiona la actualización
- Evita manipulación compleja del documento
- Mantiene compatibilidad

---

## ADR-008 — Gestión explícita de secciones

### Estado

Aceptado

### Decisión

Las secciones (`sectPr`) solo se crean cuando hay un cambio semántico real:

- portada
- TOC
- cuerpo
- anexos

No se crean secciones por comodidad.

### Justificación

- Evita documentos frágiles
- Simplifica numeración de páginas

---

## ADR-009 — Golden Sample como referencia obligatoria

### Estado

Aceptado

### Decisión

Existe un **perfil golden sample** que actúa como referencia obligatoria para:

- tests de integración
- validación visual
- regresiones

### Justificación

- Permite detectar cambios no deseados
- Facilita control de calidad

---

## Estado del documento

Este documento está vivo. Nuevas decisiones arquitectónicas deben añadirse como nuevos ADR, manteniendo el histórico.
