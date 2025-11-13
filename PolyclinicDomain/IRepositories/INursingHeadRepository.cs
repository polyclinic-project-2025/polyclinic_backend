using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks; 
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface INursingHeadRepository : IRepository<NursingHead>
{
    Task<IEnumerable<NursingHead>> GetByManagedNursingId(Guid managedNursingId);
}