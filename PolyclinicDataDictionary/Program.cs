using Microsoft.EntityFrameworkCore;
using PolyclinicInfrastructure.Persistence;
using PolyclinicInfrastructure.Tools;

namespace PolyclinicDataDictionary;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Generador de Documentación - Sistema Polyclinic    ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine();

        // Configurar DbContext
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=polyclinicDb;Username=user_medical;Password=polyclinic");
        optionsBuilder.UseLazyLoadingProxies();

        using var context = new AppDbContext(optionsBuilder.Options);

        // Crear directorio de salida
        var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Documentation");
        Directory.CreateDirectory(outputDir);

        Console.WriteLine("📚 Generando Diccionario de Datos...\n");
        
        // Generar diccionario de datos
        var dictGenerator = new DataDictionaryGenerator(context);
        await dictGenerator.SaveToFileAsync(
            Path.Combine(outputDir, "DataDictionary.md"), 
            "markdown"
        );
        Console.WriteLine("   ✓ DataDictionary.md");

        await dictGenerator.SaveToFileAsync(
            Path.Combine(outputDir, "DataDictionary.html"), 
            "html"
        );
        Console.WriteLine("   ✓ DataDictionary.html");

        Console.WriteLine();
        Console.WriteLine("📊 Generando Diagramas UML...\n");

        // Generar diagramas UML
        var umlGenerator = new UmlDiagramGenerator(context);
        var umlDir = Path.Combine(outputDir, "UML");
        await umlGenerator.SaveAllDiagramsAsync(umlDir);
        
        Console.WriteLine("   ✓ Complete_Domain_Model.puml");
        Console.WriteLine("   ✓ Simplified_Relationships.puml");
        Console.WriteLine("   ✓ Personal_Module.puml");
        Console.WriteLine("   ✓ Departments_Module.puml");
        Console.WriteLine("   ✓ Patients_Module.puml");
        Console.WriteLine("   ✓ Consultations_Module.puml");
        Console.WriteLine("   ✓ Emergency_Module.puml");
        Console.WriteLine("   ✓ Medications_Module.puml");
        Console.WriteLine("   ✓ Warehouse_Module.puml");
        Console.WriteLine("   ✓ README.md (instrucciones)");

        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.WriteLine("✅ ¡Documentación generada exitosamente!");
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine($"📁 Ubicación: {Path.GetFullPath(outputDir)}");
        Console.WriteLine();
        Console.WriteLine("Para visualizar los diagramas UML:");
        Console.WriteLine("  1. Instala la extensión 'PlantUML' en VS Code");
        Console.WriteLine("  2. Abre cualquier archivo .puml");
        Console.WriteLine("  3. Presiona Alt+D para previsualizar");
        Console.WriteLine();
        Console.WriteLine("O visita: http://www.plantuml.com/plantuml/uml/");
    }
}
