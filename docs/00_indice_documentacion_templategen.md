# Índice General de Documentación

## Proyecto: Sistema Generador de Plantillas Word (OpenXML)

---

## 1. Propósito de este documento

Este documento actúa como **índice maestro** de la documentación del proyecto *TemplateGen*, proporcionando una visión estructurada y ordenada de todos los documentos existentes, su propósito y su relación dentro del ciclo de diseño e implementación.

Debe ser el **punto de entrada único** para cualquier persona (técnica o funcional) que se incorpore al proyecto.

Ubicación recomendada:

- `/docs/00_indice_documentacion_templategen.md`

---

## 2. Estructura de la carpeta `/docs`

La carpeta `/docs` contiene la documentación viva del proyecto, organizada de forma progresiva desde la definición del problema hasta la ejecución técnica.

El orden numérico de los documentos **no es casual**: refleja el orden lógico recomendado de lectura y trabajo.

---

## 3. Índice de documentos

### 01. `01_requisitos_del_sistema_generador_de_plantillas_word.md`

**Título:** Requisitos del Sistema Generador de Plantillas Word

**Descripción:**
Define los requisitos funcionales y no funcionales del sistema. Establece el alcance, objetivos, restricciones, principios de diseño y criterios de aceptación globales. Este documento responde a la pregunta:

"¿Qué debe hacer el sistema y bajo qué condiciones?"

Es el documento base que gobierna todas las decisiones posteriores.

---

### 02. `02_esquema_del_perfil_de_cliente_template_profile_schema.md`

**Título:** Esquema del Perfil de Cliente (Template Profile Schema)

**Descripción:**
Describe la estructura lógica del perfil de cliente desde un punto de vista conceptual. Identifica los grandes bloques de configuración (documento, estilos, numeración, secciones, assets, reglas) sin entrar aún en detalles de validación formal.

Sirve como puente entre los requisitos del sistema y el modelo de dominio.

---

### 03. `03_modelo_de_dominio_del_motor_generador_open_xml.md`

**Título:** Modelo de Dominio del Motor Generador (OpenXML)

**Descripción:**
Define el modelo interno del sistema: entidades, objetos de valor, agregados, invariantes y servicios de dominio. Establece una separación estricta entre dominio e infraestructura OpenXML.

Este documento responde a:

"¿Qué conceptos existen internamente y cómo se relacionan?"

---

### 04. `04_json_schema_del_perfil_de_cliente_template_profile_v1.md`

**Título:** JSON Schema del Perfil de Cliente (Template Profile)

**Descripción:**
Especifica el contrato técnico formal del perfil de cliente mediante JSON Schema (draft 2020-12). Permite la validación automática de perfiles antes de la generación y define tipos, campos obligatorios, restricciones y extensibilidad.

Es la base para la validación determinista del sistema.

---

### 05. `05_arquitectura_de_implementacion_del_motor_open_xml.md`

**Título:** Arquitectura de Implementación del Motor (OpenXML)

**Descripción:**
Detalla la arquitectura técnica del proyecto: estructura de repositorio, capas, módulos, responsabilidades, pipeline de ejecución y diseño de writers OpenXML.

Responde a:

"¿Cómo se implementa el sistema de forma mantenible y escalable?"

---

### 06. `06_perfil_de_ejemplo_golden_sample_y_estructura_de_carpeta.md`

**Título:** Perfil de Ejemplo (Golden Sample) y Estructura de Carpeta

**Descripción:**
Proporciona un perfil de cliente completo y funcional que sirve como referencia estable (golden sample) para pruebas de integración y validación visual. Incluye también la estructura de carpetas recomendada por cliente.

Este documento es clave para la PoC y los golden tests.

---

### 07. `07_plan_de_implementacion_poc_por_fases_open_xml_template_gen.md`

**Título:** Plan de Implementación PoC por Fases

**Descripción:**
Define un plan incremental de implementación dividido en fases con objetivos, tareas, entregables y criterios de aceptación claros. Minimiza riesgos técnicos abordando la complejidad de forma progresiva.

Guía el desarrollo desde cero hasta una PoC funcional completa.

---

### 08. `08_convenciones_de_codigo_y_decisiones_open_xml.md`

**Título:** Convenciones de Código y Decisiones OpenXML

**Descripción:**
Establece las reglas normativas de implementación: convenciones de código, decisiones OpenXML obligatorias, principios de determinismo, antipatrones prohibidos y criterios técnicos que deben respetarse durante el desarrollo.

Este documento evita desviaciones técnicas difíciles de corregir.

---

### 09. `09_checklist_diario_de_implementacion_template_gen_open_xml.md`

**Título:** Checklist Diario de Implementación

**Descripción:**
Proporciona un checklist operativo para el día a día del desarrollo. Cubre controles antes de empezar una tarea, durante la implementación, validaciones, pruebas, PRs, merges y cierre diario.

Su objetivo es mantener calidad, foco y trazabilidad continua.

---

### 10. `ARCHITECTURE_DECISIONS.md`

**Título:** Architecture Decision Records (ADR)

**Descripción:**  
Registra las decisiones arquitectónicas clave del proyecto y su justificación.  
Explica por qué el sistema es como es, qué alternativas se descartaron y qué
consecuencias tienen dichas decisiones.

Actúa como memoria arquitectónica y protege el diseño frente a refactors no
alineados con los principios del proyecto.

---
    
## 4. Estado de la documentación

Con este índice, la documentación del proyecto se considera:

- Completa a nivel de diseño
- Coherente y ordenada
- Lista para iniciar implementación sin improvisación

Este documento deberá actualizarse si se añaden nuevos documentos o si alguno cambia de alcance de forma significativa.
