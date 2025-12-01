namespace PolyclinicApplication.DTOs.Request.Patients
{
    public class CreatePatientDto
    {
        public string Name { get; set; } = string.Empty;
        public string Identification { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Contact { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
