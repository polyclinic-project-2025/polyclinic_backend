using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.DTOs.Request.Export;
using PolyclinicApplication.DTOs.Response.Export;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.Services.Interfaces.Analytics;
using PolyclinicApplication.ReadModels;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IUnifiedConsultationService _service;
    private readonly IMedicationConsumptionService _medicationConsumptionService;
    private readonly IDeniedWarehouseRequestsService _deniedWarehouseRequestsService;
    private readonly IDoctorMonthlyAverageService _doctorMonthlyAverageService;
    private readonly IDoctorSuccessRateService _doctorSuccessRateService;
    private readonly IPatientListService _patientListService;

    private readonly IExportService _exportService;

    public AnalyticsController(
        IUnifiedConsultationService service,
        IMedicationConsumptionService medicationConsumptionService,
        IDeniedWarehouseRequestsService deniedWarehouseRequestsService,
        IDoctorMonthlyAverageService doctorMonthlyAverageService,
        IDoctorSuccessRateService doctorSuccessRateService,
        IExportService exportService,
        IPatientListService patientListService)
    {
        _service = service;
        _medicationConsumptionService = medicationConsumptionService;
        _deniedWarehouseRequestsService = deniedWarehouseRequestsService;
        _doctorMonthlyAverageService = doctorMonthlyAverageService;
        _doctorSuccessRateService = doctorSuccessRateService;
        _exportService = exportService;
        _patientListService = patientListService;
    }

    // GET: api/Analytics/last10/{patientId}
    [HttpGet("last10/{patientId:guid}")]
    public async Task<IActionResult> GetLast10(Guid patientId)
    {
        var result = await _service.GetLast10ByPatientIdAsync(patientId);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<UnifiedConsultationDto>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<UnifiedConsultationDto>>.Ok(result.Value!, "Últimas consultas obtenidas"));
    }

    // GET: api/Analytics/range?patientId=...&startDate=...&endDate=...
    [HttpGet("range")]
    public async Task<IActionResult> GetByRange(
        [FromQuery] Guid patientId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var result = await _service.GetByDateRangeAsync(patientId, startDate, endDate);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<UnifiedConsultationDto>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<UnifiedConsultationDto>>.Ok(result.Value!, "Consultas en rango obtenidas"));
    }

    // GET: api/Analytics/medication-consumption?medicationId=...&month=...&year=...
    [HttpGet("medication-consumption")]
    public async Task<IActionResult> GetMedicationConsumption(
        [FromQuery] Guid medicationId,
        [FromQuery] int month,
        [FromQuery] int year)
    {
        var result = await _medicationConsumptionService.GetMonthlyConsumptionAsync(medicationId, month, year);
        
        if (!result.IsSuccess)
            return BadRequest(ApiResult<MedicationConsumptionReadModel>.Error(result.ErrorMessage!));

        return Ok(ApiResult<MedicationConsumptionReadModel>.Ok(result.Value!, "Consumo de medicamento obtenido"));
    }

    // GET: api/Analytics/denied-warehouse-requests?status=...
    [HttpGet("denied-warehouse-requests")]
    public async Task<IActionResult> GetDeniedWarehouseRequests(
        [FromQuery] string status)
    {
        var data = await _deniedWarehouseRequestsService
            .GetDeniedWarehouseRequestsAsync(status);

        return Ok(ApiResult<IEnumerable<DeniedWarehouseRequestReadModel>>
            .Ok(data.Value, "Solicitudes de almacén denegadas obtenidas"));
    }

    // GET: api/Analytics/denied-warehouse-requests/pdf?status=...
    [HttpGet("denied-warehouse-requests/pdf")]
    public async Task<ActionResult<ApiResult<ExportResponse>>> GetDeniedWarehouseRequestsPdf(
        [FromQuery] string status)
    {
        var dataResult = await _deniedWarehouseRequestsService
            .GetDeniedWarehouseRequestsAsync(status);

        if (!dataResult.IsSuccess)
        {
            return BadRequest(ApiResult<ExportResponse>.Error(dataResult.ErrorMessage!));
        }

        var exportDto = new ExportDto
        {
            Format = "pdf",
            Fields = new List<string>
            {
                "DepartmentName",
                "DepartmentHeadName",
                "Medications"
            },
            Data = dataResult.Value ,
            Name = "Solicitudes denegadas por jefe de almacén"
        };

        var exportResult = await _exportService.ExportDataAsync(exportDto);

        if (!exportResult.IsSuccess)
        {
            return BadRequest(ApiResult<ExportResponse>.Error(exportResult.ErrorMessage!));
        }

        return Ok(ApiResult<ExportResponse>
                .Ok(exportResult.Value!, 
                    "Solicitudes denegadas exportadas exitosamente"));
    }


    // GET: api/Analytics/doctor-monthly-average?from=...&to=...
    [HttpGet("doctor-monthly-average")]
    public async Task<IActionResult> GetDoctorMonthlyAverage(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var fromUtc = DateTime.SpecifyKind(from, DateTimeKind.Utc);
        var toUtc = DateTime.SpecifyKind(to, DateTimeKind.Utc);

        var data = await _doctorMonthlyAverageService
            .GetDoctorAverageAsync(fromUtc, toUtc);

        return Ok(ApiResult<IEnumerable<DoctorMonthlyAverageReadModel>>
            .Ok(data.Value, "Promedio mensual por doctor obtenido"));
    }

    // GET: api/Analytics/doctor-monthly-average/pdf?from=...&to=...
    [HttpGet("doctor-monthly-average/pdf")]
    public async Task<ActionResult<ApiResult<ExportResponse>>> GetDoctorMonthlyAveragePdf(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var fromUtc = DateTime.SpecifyKind(from, DateTimeKind.Utc);
        var toUtc = DateTime.SpecifyKind(to, DateTimeKind.Utc);

        var dataResult = await _doctorMonthlyAverageService
            .GetDoctorAverageAsync(fromUtc, toUtc);

        if (!dataResult.IsSuccess)
        {
            return BadRequest(ApiResult<ExportResponse>.Error(dataResult.ErrorMessage!));
        }

        var exportDto = new ExportDto
        {
            Format = "pdf",
            Fields = new List<string>
            {
                "DoctorName",
                "DepartmentName",
                "ConsultationAverage",
                "EmergencyRoomAverage"
            },
            Data = dataResult.Value,
            Name = "Promedio mensual de atenciones por doctor"
        };

        var exportResult = await _exportService.ExportDataAsync(exportDto);

        if (!exportResult.IsSuccess)
        {
            return BadRequest(ApiResult<ExportResponse>.Error(exportResult.ErrorMessage!));
        }

        return Ok(ApiResult<ExportResponse>
                .Ok(exportResult.Value!,
                    "Promedio mensual por doctor exportado exitosamente"));
    }


    // GET: api/Analytics/doctor-success-rate?frequency=...
    [HttpGet("doctor-success-rate")]
    public async Task<IActionResult> GetDoctorSuccessRate(
        [FromQuery] int frequency = 1)
    {
        var data = await _doctorSuccessRateService
            .GetTop5DoctorsSuccessRateAsync(frequency);

        return Ok(ApiResult<IEnumerable<DoctorSuccessRateReadModel>>
            .Ok(data.Value, "Tasa de éxito de doctores obtenida"));
    }

    // GET: api/Analytics/doctor-success-rate/pdf?frequency=...
    [HttpGet("doctor-success-rate/pdf")]
    public async Task<ActionResult<ApiResult<ExportResponse>>> GetDoctorSuccessRatePdf(
        [FromQuery] int frequency = 1)
    {
        var dataResult = await _doctorSuccessRateService
            .GetTop5DoctorsSuccessRateAsync(frequency);

        if (!dataResult.IsSuccess)
        {
            return BadRequest(ApiResult<ExportResponse>.Error(dataResult.ErrorMessage!));
        }

        var exportDto = new ExportDto
        {
            Format = "pdf",
            Fields = new List<string>
            {
                "DoctorName",
                "DepartmentName",
                "SuccessRate",
                "TotalPrescriptions",
                "FrequentMedications"
            },
            Data = dataResult.Value,
            Name = "Tasa de éxito de prescripciones por doctor"
        };

        var exportResult = await _exportService.ExportDataAsync(exportDto);

        if (!exportResult.IsSuccess)
        {
            return BadRequest(ApiResult<ExportResponse>.Error(exportResult.ErrorMessage!));
        }

        return Ok(ApiResult<ExportResponse>
                .Ok(exportResult.Value!,
                    "Tasa de éxito de doctores exportada exitosamente"));
    }
    
    // GET: api/Analytics/patients-list
    [HttpGet("patients-list")]
    public async Task<IActionResult> GetPatientsList()
    {
        var data = await _patientListService.GetPatientsListAsync();
    
        if (!data.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<PatientListReadModel>>.Error(data.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<PatientListReadModel>>
            .Ok(data.Value, "Lista de pacientes obtenida exitosamente"));
    }
}