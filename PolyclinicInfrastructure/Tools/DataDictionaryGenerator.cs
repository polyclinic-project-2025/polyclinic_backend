using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Tools;

/// <summary>
/// Generador de Diccionario de Datos para el sistema Polyclinic
/// </summary>
public class DataDictionaryGenerator
{
    private readonly AppDbContext _context;

    public DataDictionaryGenerator(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Genera el diccionario de datos completo en formato Markdown
    /// </summary>
    public string GenerateMarkdown()
    {
        var sb = new StringBuilder();
        
        // Título principal
        sb.AppendLine("# Diccionario de Datos - Sistema Polyclinic");
        sb.AppendLine();
        sb.AppendLine($"**Generado:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        // Tabla de contenidos
        sb.AppendLine("## Tabla de Contenidos");
        sb.AppendLine();
        
        var entityTypes = _context.Model.GetEntityTypes()
            .Where(e => !e.ClrType.Namespace!.Contains("Microsoft.AspNetCore.Identity"))
            .OrderBy(e => e.GetTableName())
            .ToList();

        foreach (var entity in entityTypes)
        {
            var tableName = entity.GetTableName();
            sb.AppendLine($"- [{tableName}](#{tableName?.ToLower()})");
        }
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        // Detalles de cada entidad
        foreach (var entity in entityTypes)
        {
            GenerateEntityDocumentation(sb, entity);
        }

        // Diagrama de relaciones
        GenerateRelationshipsSummary(sb, entityTypes);

        return sb.ToString();
    }

    /// <summary>
    /// Genera documentación para una entidad específica
    /// </summary>
    private void GenerateEntityDocumentation(StringBuilder sb, IEntityType entity)
    {
        var tableName = entity.GetTableName();
        var schema = entity.GetSchema() ?? "public";

        sb.AppendLine($"## {tableName}");
        sb.AppendLine();
        sb.AppendLine($"**Clase:** `{entity.ClrType.Name}`");
        sb.AppendLine($"**Esquema:** `{schema}`");
        sb.AppendLine($"**Tabla:** `{tableName}`");
        sb.AppendLine();

        // Descripción de la entidad (puedes personalizar esto)
        sb.AppendLine("### Descripción");
        sb.AppendLine(GetEntityDescription(entity.ClrType.Name));
        sb.AppendLine();

        // Columnas
        sb.AppendLine("### Columnas");
        sb.AppendLine();
        sb.AppendLine("| Columna | Tipo | Nullable | PK | FK | Descripción |");
        sb.AppendLine("|---------|------|----------|----|----|-------------|");

        // Ordenar primero por PK, luego alfabéticamente
        var properties = entity.GetProperties()
            .OrderByDescending(p => p.IsPrimaryKey())
            .ThenBy(p => p.Name)
            .ToList();

        foreach (var property in properties)
        {
            var columnName = property.GetColumnName();
            var columnType = property.GetColumnType();
            var isNullable = property.IsNullable ? "Sí" : "No";
            var isPrimaryKey = property.IsPrimaryKey() ? "✓" : "";
            var isForeignKey = property.IsForeignKey() ? "✓" : "";
            var maxLength = property.GetMaxLength();
            
            var description = GetPropertyDescription(entity.ClrType.Name, property.Name);
            if (maxLength.HasValue)
            {
                description += $" (Max: {maxLength})";
            }

            sb.AppendLine($"| {columnName} | {columnType} | {isNullable} | {isPrimaryKey} | {isForeignKey} | {description} |");
        }
        sb.AppendLine();

        // Claves primarias
        var primaryKey = entity.FindPrimaryKey();
        if (primaryKey != null)
        {
            sb.AppendLine("### Clave Primaria");
            sb.AppendLine();
            sb.AppendLine($"- **Nombre:** `{primaryKey.GetName()}`");
            sb.AppendLine($"- **Columnas:** {string.Join(", ", primaryKey.Properties.Select(p => $"`{p.GetColumnName()}`"))}");
            sb.AppendLine();
        }

        // Claves foráneas
        var foreignKeys = entity.GetForeignKeys().ToList();
        if (foreignKeys.Any())
        {
            sb.AppendLine("### Claves Foráneas");
            sb.AppendLine();
            
            foreach (var fk in foreignKeys)
            {
                var principalTable = fk.PrincipalEntityType.GetTableName();
                var deleteAction = fk.DeleteBehavior.ToString();
                var fkColumns = string.Join(", ", fk.Properties.Select(p => $"`{p.GetColumnName()}`"));
                var principalColumns = string.Join(", ", fk.PrincipalKey.Properties.Select(p => $"`{p.GetColumnName()}`"));

                sb.AppendLine($"- **{fk.GetConstraintName()}**");
                sb.AppendLine($"  - Columnas: {fkColumns}");
                sb.AppendLine($"  - Referencia: `{principalTable}` ({principalColumns})");
                sb.AppendLine($"  - Acción al eliminar: `{deleteAction}`");
                sb.AppendLine();
            }
        }

        // Índices
        var primaryKeyProperties = primaryKey?.Properties.Select(p => p.Name).ToHashSet() ?? new HashSet<string>();
        var indexes = entity.GetIndexes()
            .Where(i => !i.Properties.All(p => primaryKeyProperties.Contains(p.Name)))
            .ToList();
        
        if (indexes.Any())
        {
            sb.AppendLine("### Índices");
            sb.AppendLine();
            
            foreach (var index in indexes)
            {
                var indexName = index.GetDatabaseName();
                var indexColumns = string.Join(", ", index.Properties.Select(p => $"`{p.GetColumnName()}`"));
                var isUnique = index.IsUnique ? "Único" : "No único";

                sb.AppendLine($"- **{indexName}**");
                sb.AppendLine($"  - Columnas: {indexColumns}");
                sb.AppendLine($"  - Tipo: {isUnique}");
                sb.AppendLine();
            }
        }

        // Relaciones
        var navigations = entity.GetNavigations().ToList();
        if (navigations.Any())
        {
            sb.AppendLine("### Relaciones");
            sb.AppendLine();
            
            foreach (var nav in navigations)
            {
                var targetEntity = nav.TargetEntityType.GetTableName();
                var cardinality = nav.IsCollection ? "Uno a Muchos" : "Muchos a Uno";
                
                sb.AppendLine($"- **{nav.Name}** → `{targetEntity}` ({cardinality})");
            }
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine();
    }

    /// <summary>
    /// Genera un resumen de todas las relaciones
    /// </summary>
    private void GenerateRelationshipsSummary(StringBuilder sb, List<IEntityType> entities)
    {
        sb.AppendLine("## Resumen de Relaciones");
        sb.AppendLine();
        sb.AppendLine("| Tabla Origen | Relación | Tabla Destino | Tipo |");
        sb.AppendLine("|--------------|----------|---------------|------|");

        foreach (var entity in entities)
        {
            foreach (var fk in entity.GetForeignKeys())
            {
                var sourceTable = entity.GetTableName();
                var targetTable = fk.PrincipalEntityType.GetTableName();
                var relationship = string.Join(", ", fk.Properties.Select(p => p.GetColumnName()));
                var type = fk.IsUnique ? "1:1" : "N:1";

                sb.AppendLine($"| {sourceTable} | {relationship} | {targetTable} | {type} |");
            }
        }
        sb.AppendLine();
    }

    /// <summary>
    /// Obtiene descripción personalizada para cada entidad
    /// </summary>
    private string GetEntityDescription(string entityName)
    {
        return entityName switch
        {
            "Patient" => "Almacena información de los pacientes del sistema.",
            "Doctor" => "Registra datos de los médicos empleados.",
            "Department" => "Catálogo de departamentos médicos.",
            "Derivation" => "Registra derivaciones entre departamentos internos.",
            "Referral" => "Registra remisiones desde puestos médicos externos.",
            "ConsultationReferral" => "Consultas realizadas a pacientes remitidos.",
            "ConsultationDerivation" => "Consultas realizadas a pacientes derivados.",
            "DepartmentHead" => "Asignación de jefes de departamento.",
            "Nurse" => "Registra datos de enfermeros empleados.",
            "Employee" => "Tabla base para todos los empleados del sistema.",
            "Medication" => "Catálogo de medicamentos disponibles.",
            "Warehouse" => "Almacenes de medicamentos.",
            "ExternalMedicalPost" => "Puestos médicos externos que remiten pacientes.",
            "EmergencyRoom" => "Guardias de emergencias asignadas a médicos.",
            "EmergencyRoomCare" => "Atenciones realizadas en emergencias.",
            _ => "Entidad del sistema Polyclinic."
        };
    }

    /// <summary>
    /// Obtiene descripción personalizada para cada propiedad
    /// </summary>
    private string GetPropertyDescription(string entityName, string propertyName)
    {
        // Descripciones genéricas por patrón de nombre
        if (propertyName.EndsWith("Id") && propertyName.Length > 2)
            return $"Identificador de {propertyName[..^2]}";
        
        if (propertyName == "Name")
            return "Nombre";
        
        if (propertyName == "Identification")
            return "Número de identificación";
        
        if (propertyName == "CreatedAt")
            return "Fecha de creación";
        
        if (propertyName == "UpdatedAt")
            return "Fecha de última actualización";

        // Descripciones específicas
        return (entityName, propertyName) switch
        {
            ("Patient", "Age") => "Edad del paciente",
            ("Patient", "Contact") => "Información de contacto",
            ("Patient", "Address") => "Dirección de residencia",
            ("Employee", "EmploymentStatus") => "Estado laboral del empleado",
            ("ConsultationReferral", "Diagnosis") => "Diagnóstico de la consulta",
            ("ConsultationReferral", "DateTimeCRem") => "Fecha y hora de la consulta",
            ("Derivation", "DateTimeDer") => "Fecha y hora de la derivación",
            ("Referral", "DateTimeRem") => "Fecha y hora de la remisión",
            ("Medication", "ExpirationDate") => "Fecha de vencimiento del medicamento",
            ("Medication", "BatchNumber") => "Número de lote",
            ("Medication", "QuantityA") => "Cantidad disponible en almacén",
            ("Medication", "QuantityNurse") => "Cantidad disponible en enfermería",
            _ => propertyName
        };
    }

    /// <summary>
    /// Genera el diccionario en formato HTML
    /// </summary>
    public string GenerateHtml()
    {
        var markdown = GenerateMarkdown();
        // Aquí podrías usar una librería como Markdig para convertir Markdown a HTML
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Diccionario de Datos - Polyclinic</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        table {{ border-collapse: collapse; width: 100%; margin: 20px 0; }}
        th, td {{ border: 1px solid #ddd; padding: 12px; text-align: left; }}
        th {{ background-color: #4CAF50; color: white; }}
        tr:nth-child(even) {{ background-color: #f2f2f2; }}
        h1 {{ color: #333; }}
        h2 {{ color: #4CAF50; border-bottom: 2px solid #4CAF50; padding-bottom: 10px; }}
        h3 {{ color: #666; }}
        code {{ background-color: #f4f4f4; padding: 2px 6px; border-radius: 3px; }}
    </style>
</head>
<body>
    <pre>{System.Web.HttpUtility.HtmlEncode(markdown)}</pre>
</body>
</html>";
    }

    /// <summary>
    /// Guarda el diccionario en un archivo
    /// </summary>
    public async Task SaveToFileAsync(string filePath, string format = "markdown")
    {
        string content = format.ToLower() switch
        {
            "html" => GenerateHtml(),
            _ => GenerateMarkdown()
        };

        await File.WriteAllTextAsync(filePath, content);
    }
}

/// <summary>
/// Clase de programa de consola para ejecutar el generador
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        // Configurar DbContext
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=polyclinicDb;Username=user_medical;Password=polyclinic");

        using var context = new AppDbContext(optionsBuilder.Options);

        // Generar diccionario
        var generator = new DataDictionaryGenerator(context);
        
        // Guardar en Markdown
        await generator.SaveToFileAsync("DataDictionary.md", "markdown");
        Console.WriteLine("✓ Diccionario generado: DataDictionary.md");
        
        // Guardar en HTML
        await generator.SaveToFileAsync("DataDictionary.html", "html");
        Console.WriteLine("✓ Diccionario generado: DataDictionary.html");

        // También imprimir en consola
        Console.WriteLine("\n" + generator.GenerateMarkdown());
    }
}