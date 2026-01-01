# Convenciones de Código y Decisiones OpenXML

## Sistema Generador de Plantillas Word (OpenXML)

---

## 1. Propósito del documento

Este documento fija las **convenciones técnicas y decisiones OpenXML** que deben respetarse durante la implementación del motor generador de plantillas Word. Su objetivo es:

- Evitar ambigüedades técnicas
- Garantizar determinismo y reproducibilidad
- Unificar criterios entre desarrolladores
- Prevenir desviaciones difíciles de corregir a posteriori

Este documento es **normativo** para el proyecto.

---

## 2. Principios generales de implementación

1. **OpenXML explícito**: no usar atajos ni helpers que oculten estructura.
2. **Determinismo**: mismo perfil + mismo motor = mismo resultado binario (en el mismo entorno).
3. **Sin heurísticas**: el motor no “adivina” valores.
4. **Dominio antes que OpenXML**: las decisiones se toman en el dominio, no en los writers.
5. **Inmutabilidad post-composición**: el TemplateModel no se modifica durante el render.

---

## 3. Convenciones de código (C#)

### 3.1 Nombres y estructura

- Clases: PascalCase
- Métodos públicos: PascalCase
- Métodos privados: camelCase
- Variables locales: camelCase
- Constantes: PascalCase

Ejemplos:
- TemplateGenerationService
- StylesPartWriter
- ResolveStyleInheritance()

---

### 3.2 Inmutabilidad

- Objetos de dominio deben ser inmutables tras construcción
- Usar constructores completos o factories
- Evitar setters públicos

---

### 3.3 Manejo de errores

- No lanzar excepciones genéricas
- Usar tipos de error estructurados:
  - ErrorCode
  - Message
  - JsonPath
  - Severity

- Excepciones solo para errores no recuperables

---

## 4. Convenciones OpenXML generales

### 4.1 Unidades

- Twips para layout (OpenXML nativo)
- Points solo en la capa de dominio
- Conversión explícita en writers

---

### 4.2 Idioma y compatibilidad

- Definir idioma en DocumentSettingsPart
- Usar compatibilidad moderna (Word 2016+)
- No introducir elementos deprecated

---

## 5. Estilos (styles.xml)

### 5.1 Principios

- Todos los estilos se definen en styles.xml
- No se crean estilos implícitos
- `Normal` siempre existe y es el estilo por defecto

---

### 5.2 Estilo Normal

Convenciones:
- type="paragraph"
- default="1"
- No numeración
- No sangrías especiales

Debe incluir:
- run properties (fuente base)
- paragraph properties (interlineado base)

---

### 5.3 Herencia de estilos

- Usar `<w:basedOn>`
- Resolver herencia en dominio
- Escribir estilos ya materializados

No se permite:
- Ciclos
- Dependencias implícitas

---

### 5.4 Estilos de carácter

- type="character"
- Sin propiedades de párrafo
- No numeración

---

## 6. Numeración (numbering.xml)

### 6.1 Principios

- Un esquema por tipo (títulos, listas)
- Numeración definida solo aquí
- No numeración embebida en estilos

---

### 6.2 Títulos (multinivel)

Convenciones:
- Un `<w:abstractNum>` para títulos
- Un `<w:num>` asociado
- Un nivel por Title1..TitleN

Cada nivel debe definir:
- start
- numFmt
- lvlText
- pPr (indentación)

---

### 6.3 Listas

- Esquemas separados de títulos
- Reinicio controlado por `restart_behavior`

---

## 7. Documento principal (document.xml)

### 7.1 Estructura mínima

- `<w:document>`
- `<w:body>`
- Secuencias de `<w:p>` vacíos para secciones

---

### 7.2 Secciones

- Crear `<w:sectPr>` solo cuando sea necesario
- Una sección por bloque lógico (cover, toc, body, annexes)
- No crear secciones innecesarias

---

## 8. Encabezados y pies

### 8.1 Convenciones

- Un header/footer por sección
- Asociar mediante relaciones explícitas

---

### 8.2 Numeración de página

- Usar campos (`PAGE`)
- Definir formato (roman/arabic) en sectPr

---

## 9. Tabla de contenidos (TOC)

### 9.1 Inserción

- Insertar campo TOC en document.xml
- No generar índice estático

Ejemplo conceptual:
- TOC \o "1-3" \h \z \u

---

### 9.2 Actualización

- El motor no actualiza el TOC
- Word lo actualiza al abrir o manualmente

---

## 10. Assets (imágenes)

### 10.1 Inserción

- Crear ImagePart
- Registrar relación
- Insertar Drawing en header/footer o body

---

### 10.2 Convenciones

- No escalar por heurística
- Tamaños definidos explícitamente

---

## 11. Relaciones (rels)

### 11.1 Principios

- Cada relación debe ser explícita
- No asumir relaciones implícitas

---

## 12. Determinismo

Para garantizar determinismo:

- Orden estable de escritura de partes
- Orden estable de estilos y numeración
- Evitar timestamps internos
- No usar GUIDs aleatorios sin control

---

## 13. Testing OpenXML

### 13.1 Verificaciones mínimas

- styles.xml presente y válido
- numbering.xml presente y válido
- relaciones completas
- Word abre sin warnings

---

## 14. Antipatrones prohibidos

- Modificar OpenXML después de escribirlo
- Inferir estilos no definidos
- Crear secciones por comodidad
- Usar formato directo en sample.docx

---

## 15. Estado del documento

Este documento cierra la fase de diseño técnico. A partir de aquí, la implementación debe seguir estas convenciones. Cualquier desviación debe documentarse explícitamente y justificarse.

