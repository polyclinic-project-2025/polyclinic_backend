using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks; 
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface INurseRepository : IRepository<Nurse>
{
    Task<IEnumerable<Nurse>> GetByNursingAsync(Guid nursingId);
}