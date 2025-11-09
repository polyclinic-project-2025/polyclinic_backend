using System.ComponentModel.DataAnnotations;

public class BossDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El estado de empleo es obligatorio.")]
    public string EmploymentStatus { get; set; } = string.Empty;

    [Required(ErrorMessage = "La identificación es obligatoria.")]
    public int Identification { get; set; }

}