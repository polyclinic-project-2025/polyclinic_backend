using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces;

public interface IConsultationDerivationService
{
    //CRUD
    Task<Result<ConsultationDerivationDto>> CreateAsync(CreateConsultationDerivationDto dto);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateConsultationDerivationDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<ConsultationDerivationDto>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<ConsultationDerivationDto>>> GetAllAsync();

    //Custom
    Task<Result<IEnumerable<ConsultationDerivationDto>>> GetByDateRangeAsync(
        Guid patientId, DateTime startDate, DateTime endDate);

    Task<Result<IEnumerable<ConsultationDerivationDto>>> GetLast10ByPatientIdAsync(Guid patientId);

}