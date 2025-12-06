using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PolyclinicApplication.DTOs.Request;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators;

public class CreateNurseRequestValidator :
    CreateEmployeeRequestValidator<CreateNurseRequest>
{
    public CreateNurseRequestValidator() {}
}