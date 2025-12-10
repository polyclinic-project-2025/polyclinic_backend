using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces;

public interface IUnifiedConsultationService
{
    Task<Result<IEnumerable<UnifiedConsultationDto>>> GetLast10ByPatientIdAsync(Guid patientId);
    
    Task<Result<IEnumerable<UnifiedConsultationDto>>> GetByDateRangeAsync(
        Guid patientId, 
        DateTime startDate, 
        DateTime endDate);

}

