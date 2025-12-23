namespace PolyclinicApplication.ReadModels;

public record PatientListReadModel(
    string PatientFullName,
    string Identification,
    int Age,
    string Contact,
    string Address
);