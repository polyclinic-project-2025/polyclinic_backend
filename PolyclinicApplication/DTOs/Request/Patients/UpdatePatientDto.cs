namespace PolyclinicApplication.DTOs.Request.Patients
{
    public class UpdatePatientDto
    {
        // Campos opcionales para partial update
        public string? Name { get; set; }
        public string? Identification { get; set; }
        public int? Age { get; set; }
        public string? Contact { get; set; }
        public string? Address { get; set; }
    }
}
