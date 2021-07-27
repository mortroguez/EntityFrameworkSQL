using System;
using System.Threading.Tasks;
using EntityFrameworkSQL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EntityFrameworkSQL.Controllers
{
    [ApiController]
    [Route("api/v1/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly AdventureDbContext _db;
        public EmployeeController(AdventureDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("getEmployees")]
        public async Task<IActionResult> getEmployees()
        {
            try
            {
                var employees = await _db.Employees.ToListAsync();
                if (employees.Count > 0)
                {
                    return Ok(employees);
                }
                return Ok("No se encontraron empleados");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
        [HttpGet]
        [Route("getEmployee")]
        public async Task<IActionResult> getEmployee(int employeeId)
        {
            try
            {
                var employee = await _db.Employees.Where(x =>
                    x.BusinessEntityId == employeeId
                ).FirstOrDefaultAsync();
                if (employee != null)
                {
                    return Ok(employee);
                }
                return Ok("No se encontro ningun empleado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
        [HttpPost]
        [Route("createEmployee")]
        public async Task<IActionResult> CreateEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _db.Employees.AddAsync(employee);
                    await _db.SaveChangesAsync();
                    return Ok("Empleado agregado con exito");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        return BadRequest(ex.InnerException.ToString());
                    }
                    return BadRequest(ex.Message.ToString());
                }
            }
            return BadRequest("El modelo proporcionado no es valido");
        }
        [HttpPatch]
        [Route("updateEmployee")]
        public async Task<IActionResult> UpdateEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Employees.Update(employee);
                    await _db.SaveChangesAsync();
                    return Ok("Empleado actualizado con exito");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        return BadRequest(ex.InnerException.ToString());
                    }
                    return BadRequest(ex.Message.ToString());
                }
            }
            return BadRequest("El modelo proporcionado no es valido");
        }
        [HttpDelete]
        [Route("deleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(int? employeeId)
        {
            if (employeeId != null)
            {
                try
                {
                    if (!_db.Employees.Any(x => x.BusinessEntityId == employeeId))
                    {
                        return BadRequest("No se encontro el empleado solicitado");
                    }
                    Employee employee = new() { BusinessEntityId = (Int32)employeeId };
                    _db.Employees.Attach(employee);
                    _db.Employees.Remove(employee);
                    await _db.SaveChangesAsync();
                    return Ok("Empleado eliminado con exito");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        return BadRequest(ex.InnerException.ToString());
                    }
                    return BadRequest(ex.Message.ToString());
                }
            }
            return BadRequest("No se proporciono el Id del usuario");
        }
    }
}
