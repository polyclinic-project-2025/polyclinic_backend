using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PolyclinicApplication.DTOs.Request;

namespace PolyclinicApplication.Validators;

public abstract class CreateEmployeeRequestValidator<T> : 
    AbstractValidator<T> where T : 
    CreateEmployeeRequest
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.Identification)
            .NotEmpty()
            .WithMessage("La identificación es obligatoria.")

            .MaximumLength(20)
            .WithMessage("La identificación debe tener como máximo 20 caracteres.");
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio.")

            .MaximumLength(100)
            .WithMessage("El nombre debe tener como máximo 100 caracteres.");

        RuleFor(x => x.EmploymentStatus)
            .NotEmpty()
            .WithMessage("El estado de empleo es obligatorio.");
    }
}