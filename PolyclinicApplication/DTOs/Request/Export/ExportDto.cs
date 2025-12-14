namespace PolyclinicApplication.DTOs.Request.Export;

public record ExportDto
{
    public string Format { get; set; } = "pdf";
    public List<string> Fields { get; set; } = new List<string>();
    public object Data { get; set; } = null;
    public string Name { get; set; } = "Datos";
}
