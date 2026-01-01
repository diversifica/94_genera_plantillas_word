# Guía Técnica: Extensión de Propiedades de Estilo

Esta guía detalla los pasos necesarios para añadir nuevas propiedades de estilo (por ejemplo: color, tachado, resaltado) al motor de generación de `TemplateGen`.

## Flujo de Implementación

Para añadir una nueva propiedad, se deben modificar cuatro puntos clave del sistema. Tomaremos como ejemplo la implementación reciente de `underline`.

### 1. Actualizar el Modelo de Datos
Localización: `src/TemplateGen.Core/Models/StylesConfig.cs`

Añade la propiedad al record correspondiente (`RunPropertiesConfig` para texto o `ParagraphPropertiesConfig` para párrafos). Asegúrate de incluir el atributo `JsonPropertyName`.

```csharp
public record RunPropertiesConfig(
    ...
    [property: JsonPropertyName("underline")] string? Underline
);
```

### 2. Actualizar el Esquema JSON (Opcional pero Recomendado)
Localización: `src/TemplateGen.Core/Resources/schemas/profile_schema.json` (o similar)

Añade la nueva propiedad a la definición del esquema para habilitar la validación y el autocompletado en los editores de perfiles.

### 3. Implementar la Lógica en el Generador
Localización: `src/TemplateGen.Core/Services/StyleDefinitionsGenerator.cs`

Modifica el método `CreateParagraphStyle` para traducir la propiedad del modelo a elementos de OpenXML SDK.

```csharp
// Ejemplo de implementación para Underline
if (!string.IsNullOrEmpty(rConfig.Underline))
{
    var underlineValue = rConfig.Underline.ToLower();
    if (underlineValue != "none")
    {
        var underline = new Underline() { Val = UnderlineValues.Single };
        // Lógica de mapeo...
        rPr.Append(underline);
    }
}
```

### 4. Actualizar los Perfiles de Uso
Localización: `profiles/[tu_perfil]/profile.json`

Ya puedes utilizar la nueva propiedad en tus perfiles de configuración.

```json
"run": {
  "font_name": "Arial",
  "underline": "single"
}
```

## Recomendaciones
1. **Validación de Nulls**: Siempre usa `string.IsNullOrEmpty` o comprobaciones de `null` antes de procesar propiedades opcionales.
2. **Normalización**: Usa `.ToLower()` al comparar valores de texto del JSON para evitar errores por mayúsculas.
3. **Mapeo de OpenXML**: Consulta la documentación oficial de OpenXML SDK para encontrar los enums correctos (ej: `UnderlineValues`, `JustificationValues`).

## Verificación
Después de añadir una propiedad, siempre:
1. Ejecuta `dotnet build` para asegurar que no hay errores de tipo.
2. Genera un documento de prueba con la nueva propiedad activa.
3. Abre el archivo en Word para confirmación visual.
