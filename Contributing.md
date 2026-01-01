# Guía de Contribución

## Proyecto: TemplateGen – Sistema Generador de Plantillas Word (OpenXML)

---

## 1. Propósito de este documento

Este documento define **cómo se trabaja en este proyecto** de forma determinista, profesional y repetible.

No describe *qué hace el sistema* (eso está en `/docs`), sino **cómo debe actuar cualquier persona que contribuya al código o a la documentación**, siguiendo una metodología de nivel senior desde el primer commit.

Este documento es **normativo**.

---

## 2. Principios de trabajo

Todo trabajo en este proyecto debe cumplir:

1. Diseño antes de código
2. Cambios pequeños y controlados
3. Determinismo técnico
4. Trazabilidad total (documentos ↔ código ↔ commits)
5. Cero suposiciones implícitas

Si algo no está definido explícitamente, **no se asume**: se documenta o se pregunta.

---

## 3. Estructura del repositorio

La estructura del repositorio es fija y debe respetarse:

```
/docs        -> documentación del sistema (fuente de verdad)
/schemas     -> JSON Schema oficiales
/profiles    -> perfiles de cliente (ejemplos, golden samples)
/src         -> código fuente
/tests       -> tests unitarios e integración
```

No se añaden carpetas nuevas sin justificarlo y documentarlo.

---

## 4. Fuente de verdad (Source of Truth)

El orden de autoridad es el siguiente:

1. Documentación en `/docs`
2. JSON Schema en `/schemas`
3. Código en `/src`
4. Tests en `/tests`

El código **no redefine el sistema**: lo implementa.

---

### 4.1 Índice maestro de documentación

La documentación del proyecto se encuentra en la carpeta `/docs`.  
Además de los documentos individuales, existe un **índice maestro**:

- `00_indice_documentacion_templategen.md`

Este archivo **debe consultarse obligatoriamente** antes de trabajar con cualquier documento de `/docs`, ya que describe:

- qué documentos existen
- cuál es el propósito de cada uno
- el orden lógico de lectura y autoridad

No se debe asumir el contenido o alcance de un documento únicamente por su nombre; el índice `00_` es la referencia oficial para entender la documentación del proyecto.

---

### 4.2 Decisiones arquitectónicas (ADR)

El proyecto mantiene un registro explícito de decisiones arquitectónicas en:

- `/docs/ARCHITECTURE_DECISIONS.md`

Este documento recoge **por qué el sistema es como es**, incluyendo decisiones técnicas relevantes, alternativas descartadas y consecuencias asumidas.

Antes de proponer cambios estructurales, refactors significativos o nuevas aproximaciones técnicas, **debe consultarse obligatoriamente el ADR** para verificar si la decisión ya fue tomada o si existe criterio arquitectónico previo.

Las decisiones existentes no se modifican ni se reescriben; cualquier cambio de enfoque debe registrarse mediante un nuevo ADR.

---

## 5. Flujo de trabajo obligatorio

### 5.1 Antes de empezar cualquier trabajo

Debe cumplirse:

- La tarea está alineada con una fase del `Plan de Implementación PoC`.
- El objetivo del cambio está claramente definido.
- El criterio de aceptación está claro y es verificable.

No se empieza a programar sin esto.

---

### 5.2 Ramas

El proyecto sigue un flujo de ramas controlado para garantizar estabilidad y trazabilidad.

Convención obligatoria:

- `main`  
  Rama **estable** del proyecto.  
  Solo recibe código probado, validado y alineado con la documentación.  
  No se trabaja directamente sobre esta rama.

- `develop`  
  Rama **de integración** y trabajo activo.  
  Todas las contribuciones funcionales se integran aquí antes de pasar a `main`.

- `feature/<descripcion-corta>`  
  Ramas para nuevas funcionalidades o fases del plan PoC.  
  Se crean desde `develop` y se fusionan exclusivamente contra `develop`.

- `fix/<descripcion-corta>`  
  Ramas para correcciones de errores.  
  Se crean desde `develop` (o excepcionalmente desde `main` si el bug está en producción) y se fusionan contra `develop`.

- `docs/<descripcion-corta>`  
  Ramas para cambios exclusivamente documentales.  
  Se crean desde `develop` y se fusionan contra `develop`.

Reglas obligatorias:

- Nunca se trabaja directamente sobre `main`.
- Toda rama tiene **un único propósito**.
- Toda fusión a `develop` se realiza mediante Pull Request.
- El paso de `develop` a `main` representa un **hito estable del proyecto**.

### 5.2.1 Regla de Oro del Flujo de Trabajo (Strict Workflow Enforcement)

Para garantizar el determinismo y la trazabilidad, se debe seguir estrictamente este orden **sin excepciones**:

1.  **Issue**: No se escribe código sin una Issue abierta que lo describa.
2.  **Rama**: Se crea una rama desde `develop` (e.g., `feature/xyz`, `fix/abc`) asociada a la Issue.
3.  **Implementación**: Se codifica y se realizan commits en la rama.
4.  **Test**: Se verifica localmente (Unit Tests + Manual).
5.  **Pull Request**: Se abre un PR hacia `develop` referenciando la Issue (e.g., `Closes #N`).
6.  **Revisión y Merge**: Se revisa y se fusiona a `develop`. **Nunca se hace commit directo a `develop` o `main`**.
7.  **Siguiente Fase**: Solo tras el merge se procede a la siguiente fase o tarea.

---

### 5.3 Gestión de issues por fase

Antes de iniciar cualquier fase del proyecto (según el Plan de Implementación PoC), **deben crearse previamente las issues correspondientes en el repositorio**.

Reglas obligatorias:

- Cada fase debe tener sus issues definidas **antes de escribir código**.
- Las issues deben reflejar:
  - el objetivo de la fase o tarea
  - el alcance concreto
  - el criterio de aceptación
- No se comienza una fase sin issues asociadas.

### 5.4 Etiquetas (labels)

Las issues deben etiquetarse correctamente para permitir trazabilidad y control.

- Si las etiquetas necesarias no existen, **deben crearse previamente**.
- Etiquetas recomendadas (mínimo):
  - `fase`
  - `feature`
  - `fix`
  - `docs`
  - `arquitectura`
  - `validacion`
  - `openxml`

### 5.5 Idioma y codificación

- Las issues deben redactarse **en español**.
- El texto debe estar en **UTF-8 limpio**, evitando caracteres especiales no estándar.
- No deben aparecer caracteres corruptos, sustituciones extrañas ni problemas de codificación.

La claridad y correcta codificación del texto es obligatoria, ya que las issues forman parte de la trazabilidad del proyecto.


### 5.6 Commits

Reglas:

- Commits pequeños y autocontenidos
- Un commit no mezcla responsabilidades
- El mensaje describe *qué cambia*, no *cómo*

Formato recomendado:

```
<tipo>: <resumen>

<detalle opcional>
```

Tipos permitidos:
- feat
- fix
- refactor
- docs
- test
- chore

---

### 5.7 Pull Requests

Todo cambio entra por PR.

Un PR debe incluir:

- Objetivo claro
- Relación con fase PoC o documento
- Resultado esperado
- Tests asociados (si aplica)

Un PR no debe:
- mezclar fases
- introducir refactors no justificados
- modificar código sin actualizar documentación cuando aplica

---

## 6. Metodología de implementación

### 6.1 Diseño → Dominio → Infraestructura

Orden obligatorio:

1. Documento (si cambia el diseño)
2. Dominio (modelo, validaciones, composición)
3. Writers OpenXML (infraestructura)

Nunca al revés.

---

### 6.2 Dominio vs OpenXML

- La lógica de negocio vive en el dominio
- Los writers OpenXML **no deciden**, solo escriben
- El TemplateModel es inmutable durante el render

---

## 7. Validación obligatoria

Antes de generar cualquier artefacto:

1. Validación JSON Schema
2. Validación semántica

Si hay errores:
- No se genera salida
- El error debe ser estructurado y claro

---

## 8. Determinismo

Todo el sistema debe ser determinista:

- Mismo perfil + mismo motor = mismo resultado
- Orden estable de escritura
- Sin timestamps internos
- Sin GUIDs aleatorios no controlados

Cualquier excepción debe documentarse.

---

## 9. Testing

### 9.1 Tests unitarios

Obligatorios cuando se toca:
- validación
- herencia de estilos
- numeración

---

### 9.2 Tests de integración

Obligatorios cuando se toca:
- OpenXML
- writers
- estructura del documento

Golden sample es la referencia.

---

## 10. Documentación

### 10.1 Cuándo actualizar documentación

Se debe actualizar `/docs` cuando:

- Se añade un campo al perfil
- Se cambia una regla
- Se introduce una nueva decisión técnica

---

### 10.2 Estilo

- Markdown puro
- Lenguaje técnico, claro y conciso
- Sin ambigüedades

---

## 11. Checklist obligatorio

Antes de cerrar un PR:

- Checklist diario completado
- Golden sample genera sin errores
- Word abre los documentos sin warnings
- No se han introducido antipatrones

---

## 12. Antipatrones prohibidos

- Programar sin documento
- Inferir valores no definidos
- Lógica de negocio en writers
- Cambios grandes sin fases
- "Ya lo arreglaremos luego"

---

## 13. Resolución de dudas

Si una decisión no está clara:

1. Revisar `/docs`
2. Revisar este `CONTRIBUTING.md`
3. Documentar la duda
4. Decidir y dejar rastro

Nunca improvisar en silencio.

---

## 14. Estado del documento

Este `CONTRIBUTING.md` define la forma oficial de trabajar en TemplateGen.

Cualquier cambio en esta guía debe hacerse mediante PR y estar justificado.

