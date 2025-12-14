using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicApplication.QueryInterfaces;
using PolyclinicApplication.ReadModels;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Queries;

public class DoctorMonthlyAverageQuery : IDoctorMonthlyAverageQuery
{
    private readonly DbSet<Doctor> _dbSetDoctor;
    private readonly DbSet<ConsultationDerivation> _dbSetConsultationDerivation;
    private readonly DbSet<ConsultationReferral> _dbSetConsultationReferral;
    private readonly DbSet<EmergencyRoom> _dbSetEmergencyRoom;
    private readonly DbSet<EmergencyRoomCare> _dbSetEmergencyRoomCare;

    public DoctorMonthlyAverageQuery(AppDbContext context)
    {
        _dbSetDoctor = context.Set<Doctor>();
        _dbSetConsultationDerivation = context.Set<ConsultationDerivation>();
        _dbSetConsultationReferral = context.Set<ConsultationReferral>();
        _dbSetEmergencyRoom = context.Set<EmergencyRoom>();
        _dbSetEmergencyRoomCare = context.Set<EmergencyRoomCare>();
    }

    public async Task<IEnumerable<DoctorMonthlyAverageReadModel>> GetDoctorAverageAsync(DateTime from, DateTime to)
    {
        var derivationCounts = await _dbSetConsultationDerivation
            .Where(cd => cd.DateTimeCDer >= from && cd.DateTimeCDer <= to)
            .GroupBy(cd => cd.DoctorId)
            .Select(g => new { DoctorId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.DoctorId, x => x.Count);

        var referralCounts = await _dbSetConsultationReferral
            .Where(cr => cr.DateTimeCRem >= from && cr.DateTimeCRem <= to)
            .GroupBy(cr => cr.DoctorId)
            .Select(g => new { DoctorId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.DoctorId, x => x.Count);

        var emergencyCounts = await _dbSetEmergencyRoomCare
            .Where(ec => ec.CareDate >= from && ec.CareDate <= to)
            .Join(_dbSetEmergencyRoom,
                ec => ec.EmergencyRoomId,
                er => er.EmergencyRoomId,
                (ec, er) => new { er.DoctorId })
            .GroupBy(x => x.DoctorId)
            .Select(g => new { DoctorId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.DoctorId, x => x.Count);

        var doctors = await _dbSetDoctor.Include(d => d.Department).ToListAsync();

        var months = ((to.Year - from.Year) * 12 + to.Month - from.Month + 1);
        if (months <= 0) months = 1;

        var result = doctors.Select(d =>
        {
            derivationCounts.TryGetValue(d.EmployeeId, out var derivCount);
            referralCounts.TryGetValue(d.EmployeeId, out var refCount);
            emergencyCounts.TryGetValue(d.EmployeeId, out var emergencyCount);

            var totalConsultations = derivCount + refCount;
            var avgConsultations = (double)totalConsultations / months;
            var avgEmergency = (double)emergencyCount / months;

            return new DoctorMonthlyAverageReadModel(
                DoctorName: d.Name,
                DepartmentName: d.Department?.Name ?? string.Empty,
                ConsultationAverage: Math.Round(avgConsultations, 2),
                EmergencyRoomAverage: Math.Round(avgEmergency, 2)
            );
        }).ToList();

        return result;
    }

}