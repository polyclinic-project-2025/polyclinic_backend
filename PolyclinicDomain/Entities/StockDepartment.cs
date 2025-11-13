namespace PolyclinicDomain.Entities;
public class StockDepartment{
    public Guid Id { get; private set; }
    public Guid IdMed { get; private set;}
    public int Quantity {get;private set;}
    public Medication Medication {get;set; }
    public Department Department {get;set; }
    public StockDepartment(Guid id,Guid idMed,int quantity){
        Id = id;
        IdMed = idMed;
        Quantity = quantity;
    }
}