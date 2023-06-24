using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class EmployeesWithDepartmentsSpecification : BaseSpecifications<Employee>
    {
        public EmployeesWithDepartmentsSpecification()
        {
            Includes.Add(E => E.Department);
        }

        public EmployeesWithDepartmentsSpecification(int id):base (E => E.Id == id)
        {
            Includes.Add(E => E.Department);
        }
    }
}
