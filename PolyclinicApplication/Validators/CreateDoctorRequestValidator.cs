using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PolyclinicApplication.DTOs.Request;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators;

public class CreateDoctorRequestValidator : 
    CreateEmployeeRequestValidator<CreateDoctorRequest>
{
    public CreateDoctorRequestValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithMessage("El ID del departamento es obligatorio.");
    }
}