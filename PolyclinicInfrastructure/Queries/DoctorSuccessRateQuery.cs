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

public class DoctorSuccessRateQuery : IDoctorSuccessRateQuery
{
    private readonly DbSet<Doctor> _dbSetDoctor;
    private readonly DbSet<Patient> _dbSetPatient;
    private readonly DbSet<Derivation> _dbSetDerivation;
    private readonly DbSet<Referral> _dbSetReferral;
    private readonly DbSet<ConsultationDerivation> _dbSetConsultationDerivation;
    private readonly DbSet<ConsultationReferral> _dbSetConsultationReferral;
    private readonly DbSet<MedicationDerivation> _dbSetMedicationDerivation;
    private readonly DbSet<MedicationReferral> _dbSetMedicationReferral;
    private readonly DbSet<Medication> _dbSetMedication;

    public DoctorSuccessRateQuery(AppDbContext context)
    {
        _dbSetDoctor = context.Set<Doctor>();
        _dbSetPatient = context.Set<Patient>();
        _dbSetDerivation = context.Set<Derivation>();
        _dbSetReferral = context.Set<Referral>();
        _dbSetConsultationDerivation = context.Set<ConsultationDerivation>();
        _dbSetConsultationReferral = context.Set<ConsultationReferral>();
        _dbSetMedicationDerivation = context.Set<MedicationDerivation>();
        _dbSetMedicationReferral = context.Set<MedicationReferral>();
        _dbSetMedication = context.Set<Medication>();
    }

    public async Task<IEnumerable<DoctorSuccessRateReadModel>> GetTop5DoctorsAsync(int frequency)
    {
        // IDs patients over 60 years old
        var patientOver60 = await _dbSetPatient
            .Where(p => p.Age > 60)
            .Select(p => p.PatientId)
            .ToListAsync();

        // Medication prescriptions for target patiens in consultation derivations
        var medicationDerivationsPatientsOver60 = await _dbSetMedicationDerivation
            .Where(md => patientOver60.Contains(md.ConsultationDerivation.Derivation.PatientId))
            .Select(md => new {
                ConsultationKey = md.ConsultationDerivationId,
                DoctorId = md.ConsultationDerivation.DoctorId,
                PatientId = md.ConsultationDerivation.Derivation.PatientId,
                Date = md.ConsultationDerivation.DateTimeCDer,
                MedicationId = md.MedicationId,
                DepartmentName = md.ConsultationDerivation.Derivation.DepartmentTo.Name
            })
            .ToListAsync();

        // Medication prescriptions for target patiens in consultation referrals
        var medicationReferralsPatientsOver60 = await _dbSetMedicationReferral
            .Where(mr => patientOver60.Contains(mr.ConsultationReferral.Referral.PatientId))
            .Select(mr => new {
                ConsultationKey = mr.ConsultationReferralId,
                DoctorId = mr.ConsultationReferral.DoctorId,
                PatientId = mr.ConsultationReferral.Referral.PatientId,
                Date = mr.ConsultationReferral.DateTimeCRem,
                MedicationId = mr.MedicationId,
                DepartmentName = mr.ConsultationReferral.Referral.DepartmentTo.Name
            })
            .ToListAsync();

        // Join prescriptions
        var medicationPatientsOver60 = medicationDerivationsPatientsOver60
            .Select(e => new {
                e.ConsultationKey,
                e.DoctorId,
                e.PatientId,
                e.Date,
                e.MedicationId,
                e.DepartmentName
            })
            .Concat(medicationReferralsPatientsOver60
            .Select(e => new {
                e.ConsultationKey,
                e.DoctorId,
                e.PatientId,
                e.Date,
                e.MedicationId,
                e.DepartmentName
            }))
            .ToList();

        // Consultation derivation dates per patient
        var consultationDatesDerivations = await _dbSetConsultationDerivation
            .Where(cd => patientOver60.Contains(cd.Derivation.PatientId))
            .Select(cd => new { 
                PatientId = cd.Derivation.PatientId, 
                Date = cd.DateTimeCDer 
            })
            .ToListAsync();
        

        // Consultation referral dates per patient
        var consultationDatesReferrals = await _dbSetConsultationReferral
            .Where(cr => patientOver60.Contains(cr.Referral.PatientId))
            .Select(cr => new { 
                PatientId = cr.Referral.PatientId, 
                Date = cr.DateTimeCRem 
            })
            .ToListAsync();

        // Join dates
        var consultationDatesByPatient = consultationDatesDerivations
            .Concat(consultationDatesReferrals)
            .GroupBy(x => x.PatientId)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(x => x.Date)
                        .Distinct()
                        .ToList()
            );

        // Initial prescription written by a doctor to a patient
        var initialPrescriptionPerDoctorPatient = medicationPatientsOver60
            .GroupBy(e => new { 
                e.DoctorId, 
                e.PatientId
            })
            .Select(g => new {
                DoctorId = g.Key.DoctorId,
                PatientId = g.Key.PatientId,
                InitialDate = g.Min(x => x.Date)
            })
            .ToList();

        var successByDoctor = new Dictionary<Guid, int>();
        var patientsCountByDoctor = new Dictionary<Guid, int>();

        foreach (var ip in initialPrescriptionPerDoctorPatient)
        {
            // Counts distinct patients per doctor
            if (!patientsCountByDoctor.ContainsKey(ip.DoctorId))
                patientsCountByDoctor[ip.DoctorId] = 0;
            patientsCountByDoctor[ip.DoctorId]++;

            bool hadFollowUp = false;
            if (consultationDatesByPatient.TryGetValue(ip.PatientId, out var dates))
            {
                // 3-months window
                var windowEnd = ip.InitialDate.AddMonths(3);
                hadFollowUp = dates.Any(d => d > ip.InitialDate && d <= windowEnd);
            }

            if (!hadFollowUp)
            {
                // If the patient did not require a follow-up appointment 
                // within the 3-month window, it is considered a success. 
                if (!successByDoctor.ContainsKey(ip.DoctorId))
                    successByDoctor[ip.DoctorId] = 0;
                successByDoctor[ip.DoctorId]++;
            }
        }

        // Distinct consultations by doctor with medications
        var distinctConsultationsByDoctor = medicationPatientsOver60
            .Select(e => new { 
                e.DoctorId, 
                e.ConsultationKey
            })
            .Distinct()
            .GroupBy(x => x.DoctorId)
            .ToDictionary(
                g => g.Key, 
                g => g.Count()
            );

        // Obtain frequent medications
        // (The user defines the frequency)
        var medicationsPerDoctorPatient = medicationPatientsOver60
            .GroupBy(e => new { 
                e.DoctorId, 
                e.MedicationId
            })
            .Select(g => new { 
                DoctorId = g.Key.DoctorId, 
                MedicationId = g.Key.MedicationId, 
                DistinctPatients = g.Select(x => x.PatientId)
                                    .Distinct()
                                    .Count() 
            })
            .Where(x => x.DistinctPatients >= frequency)
            .ToList();

        // Frequent medications per doctor
        var medicationsPerDoctor = medicationsPerDoctorPatient
            .GroupBy(x => x.DoctorId)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(x => x.MedicationId)
                        .Distinct()
                        .ToList()
            );

        // Normalize medication list
        var medicationsToFetch = medicationsPerDoctor
                                .Values
                                .SelectMany(x => x)
                                .Distinct()
                                .ToList();

        // Associates the medication commercial name
        var medicationNames = await _dbSetMedication
            .Where(m => medicationsToFetch.Contains(m.MedicationId))
            .Select(m => new { 
                m.MedicationId, 
                m.CommercialName 
            })
            .ToDictionaryAsync(
                x => x.MedicationId, 
                x => x.CommercialName
            );
        
        // List of doctors
        var doctorIds = medicationPatientsOver60
                        .Select(e => e.DoctorId)
                        .Distinct()
                        .ToList();

        // Associates doctors with their name 
        // and the name of their department
        var doctors = await _dbSetDoctor
            .Where(d => doctorIds.Contains(d.EmployeeId))
            .Select(d => new {
                d.EmployeeId,
                d.Name,
                DepartmentName = d.Department.Name
            })
            .ToDictionaryAsync(
                x => x.EmployeeId, 
                x => new { 
                    x.Name, 
                    x.DepartmentName
                }
            );

        var models = new List<DoctorSuccessRateReadModel>();

        foreach (var docId in doctorIds)
        {
            patientsCountByDoctor.TryGetValue(docId, out var totalPatients);
            successByDoctor.TryGetValue(docId, out var successes);
            distinctConsultationsByDoctor.TryGetValue(docId, out var totalPresc);

            // A doctor without patients is considered 
            // to have a 0% success rate
            double rate = totalPatients > 0 ? Math.Round((double)successes / totalPatients * 100.0, 2) : 0.0;

            medicationsPerDoctor.TryGetValue(docId, out var medsIds);
            var medsNamesForDoctor = (medsIds ?? new List<Guid>())
                                        .Where(id => medicationNames.ContainsKey(id))
                                        .Select(id => medicationNames[id]);
            var medsConcatenated = string.Join(" ", medsNamesForDoctor);

            string doctorName = doctors[docId].Name;
            string departmentName = doctors[docId].DepartmentName;

            models.Add(new DoctorSuccessRateReadModel(
                doctorName,
                departmentName,
                rate,
                totalPresc,
                medsConcatenated
            ));
        }

        // Get the top 5 doctors with the highest 
        // success rate and number of prescriptions
        var top5 = models
                    .OrderByDescending(m => m.SuccessRate)
                    .ThenByDescending(m => m.TotalPrescriptions)
                    .Take(5)
                    .ToList();

        return top5;
    }
}