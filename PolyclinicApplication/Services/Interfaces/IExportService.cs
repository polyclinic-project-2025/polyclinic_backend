
using PolyclinicApplication.DTOs.Request.Export;
using PolyclinicApplication.DTOs.Response.Export;
using PolyclinicApplication.Common.Results;
namespace PolyclinicApplication.Services.Interfaces
{
    public interface IExportService
    {
        Task<Result<ExportResponse>> ExportDataAsync(ExportDto exportDto); 
    }
}