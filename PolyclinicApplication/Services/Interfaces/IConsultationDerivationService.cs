using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces;

public interface IConsutationDerivationService
{
    //CRUD
    Task<ResultT<ConsultationDerivationResponseDto>> CreateAsync(ConsultationDerivationCreateDto dto);
    Task<ResultT<ConsultationDerivationResponseDto>> UpdateAsync(Guid id, ConsultationDerivationUpdateDto dto);
    Task<ResultT<bool>> DeleteAsync(Guid id);
    Task<ResultT<ConsultationDerivationResponseDto>> GetByIdAsync(Guid id);
    Task<ResultT<IEnumerable<ConsultationDerivationResponseDto>>> GetAllAsync();

    //Custom
    Task<ResultT<IEnumerable<ConsultationDerivationResponseDto>>> GetByDateRangeAsync(
        Guid patientId, DateTime startDate, DateTime endDate);

    Task<ResultT<IEnumerable<ConsultationDerivationResponseDto>>> GetLast10ByPatientIdAsync(Guid patientId);

}