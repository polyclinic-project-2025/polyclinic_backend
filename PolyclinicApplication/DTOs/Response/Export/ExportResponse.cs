namespace PolyclinicApplication.DTOs.Response.Export
{
    public record ExportResponse
    {
        public string FilePath { get; set; }
        public string Format { get; set; }
        public string Data { get; set; }
    }
}