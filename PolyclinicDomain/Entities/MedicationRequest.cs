namespace PolyclinicDomain.Entities;

public class MedicationRequest
{
    public Guid Id { get; private set; }
    public Department? Department { get; private set; }
    public Guid DepartmentId { get; private set; }

    public Medication? Medication { get; private set; }
    public Guid MedicationId { get; private set; }

    public DateTime RequestDate { get; private set; }

    public int? Quantity { get; private set; }

    private MedicationRequest() { }
    public MedicationRequest(Guid medicationId, Guid departmentId, DateTime requestDate, int? quantity)
    {
        MedicationId = medicationId;
        DepartmentId = departmentId;
        RequestDate = requestDate;
        Quantity = quantity;
    }
}
