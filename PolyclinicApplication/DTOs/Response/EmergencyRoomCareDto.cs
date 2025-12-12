using System;

namespace PolyclinicApplication.DTOs.Response
{
    public class EmergencyRoomCareDto
    {
        public Guid EmergencyRoomCareId { get; set; }
        public string Diagnosis { get; set; }
        public Guid EmergencyRoomId { get; set; }
        public DateTime CareDate { get; set; }
        
        // Datos del Paciente
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientIdentification { get; set; }
        
        // Datos del Doctor (a través de EmergencyRoom)
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorIdentification { get; set; }
    }
}