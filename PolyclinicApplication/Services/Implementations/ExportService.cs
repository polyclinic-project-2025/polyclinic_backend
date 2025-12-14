using System.Text.Json;
using PolyclinicApplication.Common.Interfaces;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.Export;
using PolyclinicApplication.DTOs.Response.Export;
using PolyclinicApplication.Services.Interfaces;

namespace PolyclinicApplication.Services.Implementations;

public class ExportService : IExportService
{
    private readonly IExportStrategyFactory _exportStrategyFactory;

    public ExportService(IExportStrategyFactory exportStrategyFactory)
    {
        _exportStrategyFactory = exportStrategyFactory;
    }

    public async Task<Result<ExportResponse>> ExportDataAsync(ExportDto exportDto)
    {
        try
        {
            string format = exportDto.Format.ToLower();
            var dataObject = exportDto.Data;
            string name = exportDto.Name;
            List<string> columns = exportDto.Fields;

            // Serializar a JSON
            string data = JsonSerializer.Serialize(dataObject);

            // Generar archivo temporal
            string filePath = Path.Combine(Path.GetTempPath(), $"export_{Guid.NewGuid()}.pdf");

            // Crear estrategia según el formato
            var strategy = _exportStrategyFactory.CreateExportStrategy(format);

            // Exportar datos
            strategy.Export(data, filePath, name, columns);

            // Leer archivo generado y convertir a Base64
            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            string base64Data = Convert.ToBase64String(fileBytes);

            // Eliminar archivo temporal
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var response = new ExportResponse
            {
                FilePath = Path.GetFileName(filePath),
                Format = format,
                Data = base64Data
            };

            return Result<ExportResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<ExportResponse>.Failure($"Error al exportar datos: {ex.Message}");
        }
    }
}
