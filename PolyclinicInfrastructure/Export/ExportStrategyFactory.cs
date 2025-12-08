using PolyclinicApplication.Common.Interfaces;

namespace PolyclinicInfrastructure.Export
{
    public class ExportStrategyFactory : IExportStrategyFactory
    {
        public IExportStrategy CreateExportStrategy(string format)
        {
            return format.ToLower() switch
            {
                "pdf" => new PdfExportStrategy(),
                _ => throw new NotSupportedException($"The format '{format}' is not supported.")
            };
        }
    }
}