namespace PolyclinicApplication.ReadModels;

public record PatientListReadModel(
    string PatientName,
    string Identification,
    int Age,
    string Contact,
    string Address
);