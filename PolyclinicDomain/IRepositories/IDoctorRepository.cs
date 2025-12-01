using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface IDoctorRepository : IEmployeeRepository<Doctor> {}