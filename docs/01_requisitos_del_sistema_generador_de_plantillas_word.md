# Documento de Requisitos

## Sistema Generador de Plantillas Word (OpenXML)

---

## 1. Propósito del documento

Este documento define de forma exhaustiva y estricta los **requisitos funcionales y no funcionales** del sistema generador de plantillas Word basado en **OpenXML**. Su objetivo es servir como contrato técnico inicial para diseñar e implementar el sistema sin necesidad de parches estructurales posteriores.

El sistema permitirá definir, versionar y generar plantillas Word profesionales (.dotx) a partir de configuraciones declarativas por cliente, garantizando coherencia, escalabilidad y mantenibilidad.

---

## 2. Alcance del sistema

El sistema deberá:

- Generar plantillas Word profesionales completamente configuradas.
- Basarse en especificaciones declarativas externas.
- Soportar múltiples clientes con requisitos distintos.
- Permitir evolución futura sin romper compatibilidad.
- Garantizar control total sobre estilos, numeración, secciones y metadatos.

Queda fuera de alcance en esta fase:
- Edición colaborativa en tiempo real.
- Integración directa con sistemas de gestión documental externos.
- Interfaz gráfica avanzada (se contempla como fase posterior).

---

## 3. Principios de diseño

1. **Declarativo primero**: toda configuración deberá provenir de archivos de especificación.
2. **Separación estricta** entre especificación y motor.
3. **Determinismo**: misma entrada produce siempre la misma plantilla.
4. **Idempotencia**: regenerar una plantilla no introduce efectos colaterales.
5. **Trazabilidad total**: cada plantilla debe poder reconstruirse a partir de su configuración.
6. **Escalabilidad estructural**: nuevos parámetros no rompen perfiles existentes.

---

## 4. Arquitectura general

### 4.1 Componentes

- **Especificación de cliente** (configuración declarativa)
- **Motor generador OpenXML**
- **Repositorio de perfiles y versiones**
- **Sistema de validación previa**
- **Sistema de salida y artefactos**

---

## 5. Especificación de cliente

### 5.1 Formato

- Formato obligatorio: JSON o YAML
- Codificación UTF-8
- Estructura modular y extensible

### 5.2 Metadatos obligatorios

- Identificador único de cliente
- Nombre descriptivo
- Versión del perfil
- Fecha de creación
- Fecha de última modificación
- Autor o responsable

### 5.3 Configuración del documento

- Tamaño de página
- Orientación
- Márgenes (superior, inferior, izquierdo, derecho)
- Idioma principal
- Estilo de numeración de páginas

---

## 6. Gestión de estilos

### 6.1 Estilos de párrafo obligatorios

- Normal (estilo base)
- Título 1
- Título 2
- Título 3
- Lista con viñetas
- Lista numerada
- Cita
- Nota
- Ejemplo
- Código de bloque
- Pie de figura

Cada estilo deberá definir explícitamente:
- Fuente
- Tamaño
- Interlineado
- Espaciado antes y después
- Herencia (estilo base)

### 6.2 Estilos de carácter obligatorios

- Énfasis
- Énfasis fuerte
- Término
- Código en línea

Queda prohibido el uso de formato manual como sustituto de estilos.

---

## 7. Numeración y esquemas multinivel

- Definición explícita del esquema multinivel de títulos
- Asociación directa entre niveles de numeración y estilos de título
- Soporte para:
  - Numeración decimal
  - Anexos alfabéticos
- Configuración independiente de numeración de listas

---

## 8. Secciones y saltos

El sistema deberá:

- Crear secciones solo cuando sea funcionalmente necesario
- Soportar secciones diferenciadas para:
  - Portada
  - Índice
  - Cuerpo principal
  - Anexos
- Configurar encabezados y pies por sección

---

## 9. Encabezados y pies de página

Configurables por sección:

- Contenido textual
- Numeración de página
- Inclusión de logotipos
- Alineación
- Herencia entre secciones

---

## 10. Índices automáticos

El sistema deberá generar:

- Índice general automático
- Índice de figuras (opcional)
- Índice de tablas (opcional)
- Lista de acrónimos (opcional)

Configurables:
- Niveles incluidos
- Estilos asociados

---

## 11. Activos y recursos

- Gestión de imágenes (logos)
- Rutas relativas controladas
- Validación de existencia previa

---

## 12. Versionado y repositorio

El sistema deberá:

- Mantener un repositorio estructurado por cliente
- Versionar cada perfil de configuración
- Permitir reconstrucción histórica de plantillas
- Generar un changelog automático por versión

---

## 13. Validaciones obligatorias

Antes de generar una plantilla, el sistema deberá validar:

- Integridad de la especificación
- Existencia de estilos obligatorios
- Coherencia de herencias
- Correcta definición de numeración
- Compatibilidad con la versión del motor

---

## 14. Salidas del sistema

Por cada ejecución:

- Plantilla Word (.dotx)
- Documento de prueba (.docx)
- Informe de generación
- Resumen de configuración aplicada

---

## 15. Requisitos no funcionales

- Compatibilidad con Microsoft Word moderno
- Generación sin necesidad de Word instalado
- Tiempo de generación predecible
- Código mantenible y documentado
- Posibilidad de integración futura con CI/CD

---

## 16. Evolución futura

El sistema deberá permitir:

- Nuevos módulos de configuración
- Nuevos tipos de estilos
- Nuevos formatos de salida

Sin romper compatibilidad con perfiles existentes.

---

## 17. Criterio de aceptación

El sistema se considerará válido cuando:

- Una plantilla completa pueda generarse solo a partir de su especificación
- Dos ejecuciones con la misma configuración produzcan resultados idénticos
- La plantilla generada permita crear documentos sin formato manual

---

## 18. Estado del documento

Este documento define la versión inicial de requisitos y será la base para el diseño de arquitectura y la implementación técnica.

