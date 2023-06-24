using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{
    public class EmployeesController : BaseAPIsController
    {
        private readonly IGenericRepository<Employee> _employeeRepo;

        public EmployeesController(IGenericRepository<Employee> employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        //[HttpGet] // ../api/Employees
        //public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
        //{
        //    var spec = new EmployeesWithDepartmentsSpecification();
        //    var employees = await _employeeRepo.GetAllWithSpecAsync(spec);
        //    return Ok(employees);
        //}

        //[HttpGet] // ../api/Employees/1
        //public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        //{
        //    var spec = new EmployeesWithDepartmentsSpecification(id);
        //    var employee = await _employeeRepo.GetByIdWithSpecAsync(spec);
        //    return Ok(employee);
        //}
    }
}
