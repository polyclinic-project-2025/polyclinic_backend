using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PolyclinicApplication.DTOs.Request;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators;


public abstract class UpdateEmployeeRequestValidator<T> : 
    AbstractValidator<T> 
    where T : UpdateEmployeeRequest
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.Identification)
            .MaximumLength(20)
            .WithMessage("La identificación debe tener como máximo 20 caracteres.");
        
        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("El nombre debe tener como máximo 100 caracteres.");
    }
}