using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Tools;

/// <summary>
/// Generador de diagramas UML en formato PlantUML desde Entity Framework Core
/// </summary>
public class UmlDiagramGenerator
{
    private readonly AppDbContext _context;
    private readonly HashSet<string> _processedEntities = new();

    public UmlDiagramGenerator(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Genera un diagrama completo de todas las entidades
    /// </summary>
    public string GenerateCompleteDiagram()
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("@startuml Polyclinic_Domain_Model");
        sb.AppendLine();
        sb.AppendLine("' Configuración del diagrama");
        sb.AppendLine("skinparam classAttributeIconSize 0");
        sb.AppendLine("skinparam monochrome false");
        sb.AppendLine("skinparam backgroundColor #FEFEFE");
        sb.AppendLine("skinparam class {");
        sb.AppendLine("    BackgroundColor<<Entity>> LightBlue");
        sb.AppendLine("    BackgroundColor<<Identity>> LightYellow");
        sb.AppendLine("    BorderColor Black");
        sb.AppendLine("    ArrowColor Black");
        sb.AppendLine("}");
        sb.AppendLine();

        var entities = _context.Model.GetEntityTypes()
            .Where(e => !e.ClrType.Namespace!.Contains("Microsoft.AspNetCore.Identity"))
            .OrderBy(e => e.ClrType.Name)
            .ToList();

        // Generar clases
        sb.AppendLine("' ========== ENTIDADES ==========");
        sb.AppendLine();
        
        foreach (var entity in entities)
        {
            GenerateClass(sb, entity);
        }

        sb.AppendLine();
        sb.AppendLine("' ========== RELACIONES ==========");
        sb.AppendLine();

        // Generar relaciones
        foreach (var entity in entities)
        {
            GenerateRelationships(sb, entity);
        }

        sb.AppendLine();
        sb.AppendLine("@enduml");

        return sb.ToString();
    }

    /// <summary>
    /// Genera diagramas separados por módulos
    /// </summary>
    public Dictionary<string, string> GenerateModularDiagrams()
    {
        var diagrams = new Dictionary<string, string>();

        // Módulo de Personal
        diagrams["Personal"] = GenerateModuleDiagram(
            "Personal_Module",
            new[] { "Employee", "Doctor", "Nurse", "WarehouseManager", "DepartmentHead" }
        );

        // Módulo de Departamentos
        diagrams["Departments"] = GenerateModuleDiagram(
            "Departments_Module",
            new[] { "Department", "DepartmentHead", "Doctor" }
        );

        // Módulo de Pacientes
        diagrams["Patients"] = GenerateModuleDiagram(
            "Patients_Module",
            new[] { "Patient", "Derivation", "Referral", "ExternalMedicalPost" }
        );

        // Módulo de Consultas
        diagrams["Consultations"] = GenerateModuleDiagram(
            "Consultations_Module",
            new[] { "ConsultationDerivation", "ConsultationReferral", "Doctor", "Patient", "DepartmentHead" }
        );

        // Módulo de Emergencias
        diagrams["Emergency"] = GenerateModuleDiagram(
            "Emergency_Module",
            new[] { "EmergencyRoom", "EmergencyRoomCare", "Doctor", "Patient" }
        );

        // Módulo de Medicamentos
        diagrams["Medications"] = GenerateModuleDiagram(
            "Medications_Module",
            new[] { "Medication", "MedicationDerivation", "MedicationReferral", "MedicationEmergency", 
                    "MedicationRequest", "StockDepartment" }
        );

        // Módulo de Almacén
        diagrams["Warehouse"] = GenerateModuleDiagram(
            "Warehouse_Module",
            new[] { "Warehouse", "WarehouseManager", "WarehouseRequest", "MedicationRequest", "Department" }
        );

        return diagrams;
    }

    private string GenerateModuleDiagram(string moduleName, string[] entityNames)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"@startuml {moduleName}");
        sb.AppendLine();
        sb.AppendLine("skinparam classAttributeIconSize 0");
        sb.AppendLine();

        var entities = _context.Model.GetEntityTypes()
            .Where(e => entityNames.Contains(e.ClrType.Name))
            .ToList();

        foreach (var entity in entities)
        {
            GenerateClass(sb, entity);
        }

        sb.AppendLine();
        sb.AppendLine("' Relaciones");
        
        foreach (var entity in entities)
        {
            GenerateRelationships(sb, entity, entityNames);
        }

        sb.AppendLine();
        sb.AppendLine("@enduml");

        return sb.ToString();
    }

    private void GenerateClass(StringBuilder sb, IEntityType entity)
    {
        var className = entity.ClrType.Name;
        var tableName = entity.GetTableName();
        var isAbstract = entity.ClrType.IsAbstract;

        sb.AppendLine($"class {className} <<Entity>> {{");

        // Propiedades
        var properties = entity.GetProperties()
            .Where(p => !p.IsShadowProperty())
            .OrderByDescending(p => p.IsPrimaryKey())
            .ThenBy(p => p.Name)
            .ToList();

        foreach (var property in properties)
        {
            var symbol = GetPropertySymbol(property);
            var type = GetSimpleTypeName(property.ClrType);
            var nullable = property.IsNullable ? "?" : "";
            var pk = property.IsPrimaryKey() ? " <<PK>>" : "";
            var fk = property.IsForeignKey() ? " <<FK>>" : "";

            sb.AppendLine($"    {symbol} {property.Name}: {type}{nullable}{pk}{fk}");
        }

        sb.AppendLine("}");
        sb.AppendLine();
    }

    private void GenerateRelationships(StringBuilder sb, IEntityType entity, string[]? filterEntities = null)
    {
        var className = entity.ClrType.Name;

        // Relaciones de navegación
        foreach (var navigation in entity.GetNavigations())
        {
            var targetName = navigation.TargetEntityType.ClrType.Name;
            
            // Filtrar si es necesario
            if (filterEntities != null && !filterEntities.Contains(targetName))
                continue;

            var foreignKey = navigation.ForeignKey;
            var isRequired = foreignKey.IsRequired ? "1" : "0..1";
            var targetCardinality = navigation.IsCollection ? "*" : "1";

            if (navigation.IsCollection)
            {
                // Uno a muchos
                sb.AppendLine($"{className} \"1\" --> \"{targetCardinality}\" {targetName} : {navigation.Name}");
            }
            else if (!navigation.IsOnDependent)
            {
                // Inversa de uno a muchos (ya se dibuja desde el otro lado)
                continue;
            }
        }
    }

    private string GetPropertySymbol(IProperty property)
    {
        if (property.IsPrimaryKey())
            return "{field}";
        
        return "-";
    }

    private string GetSimpleTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(Nullable<>))
            {
                return GetSimpleTypeName(type.GetGenericArguments()[0]);
            }
            if (genericType == typeof(ICollection<>))
            {
                return $"ICollection<{GetSimpleTypeName(type.GetGenericArguments()[0])}>";
            }
        }

        return type.Name switch
        {
            "String" => "string",
            "Int32" => "int",
            "Int64" => "long",
            "Decimal" => "decimal",
            "Boolean" => "bool",
            "DateTime" => "DateTime",
            "DateOnly" => "DateOnly",
            "Guid" => "Guid",
            _ => type.Name
        };
    }

    /// <summary>
    /// Genera diagrama de relaciones simplificado (solo conexiones)
    /// </summary>
    public string GenerateSimplifiedRelationshipDiagram()
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("@startuml Polyclinic_Relationships");
        sb.AppendLine();
        sb.AppendLine("' Diagrama simplificado mostrando solo relaciones");
        sb.AppendLine("skinparam classAttributeIconSize 0");
        sb.AppendLine("hide members");
        sb.AppendLine();

        var entities = _context.Model.GetEntityTypes()
            .Where(e => !e.ClrType.Namespace!.Contains("Microsoft.AspNetCore.Identity"))
            .ToList();

        // Solo nombres de clases
        foreach (var entity in entities)
        {
            sb.AppendLine($"class {entity.ClrType.Name}");
        }

        sb.AppendLine();

        // Solo relaciones
        foreach (var entity in entities)
        {
            foreach (var fk in entity.GetForeignKeys())
            {
                var source = entity.ClrType.Name;
                var target = fk.PrincipalEntityType.ClrType.Name;
                
                sb.AppendLine($"{source} --> {target}");
            }
        }

        sb.AppendLine();
        sb.AppendLine("@enduml");

        return sb.ToString();
    }

    /// <summary>
    /// Guarda todos los diagramas en archivos
    /// </summary>
    public async Task SaveAllDiagramsAsync(string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);

        // Diagrama completo
        var completeDiagram = GenerateCompleteDiagram();
        await File.WriteAllTextAsync(
            Path.Combine(outputDirectory, "Complete_Domain_Model.puml"), 
            completeDiagram
        );

        // Diagrama simplificado
        var simplifiedDiagram = GenerateSimplifiedRelationshipDiagram();
        await File.WriteAllTextAsync(
            Path.Combine(outputDirectory, "Simplified_Relationships.puml"), 
            simplifiedDiagram
        );

        // Diagramas modulares
        var modularDiagrams = GenerateModularDiagrams();
        foreach (var (moduleName, diagram) in modularDiagrams)
        {
            await File.WriteAllTextAsync(
                Path.Combine(outputDirectory, $"{moduleName}_Module.puml"), 
                diagram
            );
        }

        // Crear archivo README con instrucciones
        var readme = @"# Diagramas UML - Sistema Polyclinic

## Archivos Generados

- **Complete_Domain_Model.puml**: Diagrama completo con todas las entidades
- **Simplified_Relationships.puml**: Diagrama simplificado mostrando solo relaciones
- **Módulos**: Diagramas separados por módulo funcional

## Cómo Visualizar

### Opción 1: PlantUML Online
1. Ve a http://www.plantuml.com/plantuml/uml/
2. Copia y pega el contenido de cualquier archivo .puml

### Opción 2: Visual Studio Code
1. Instala la extensión ""PlantUML""
2. Abre cualquier archivo .puml
3. Presiona Alt+D para previsualizar

### Opción 3: Exportar a imagen
```bash
# Instalar PlantUML (requiere Java)
java -jar plantuml.jar Complete_Domain_Model.puml

# O usando la extensión de VS Code
# Click derecho > PlantUML: Export Current Diagram
```

## Formatos de Exportación Disponibles
- PNG (imágenes)
- SVG (vectorial, escalable)
- PDF (documentación)
";
        await File.WriteAllTextAsync(
            Path.Combine(outputDirectory, "README.md"), 
            readme
        );
    }
}