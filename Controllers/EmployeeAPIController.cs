using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffManagementN.Data;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            var employeeDtos = employees.Select(e => new EmployeeDto
            {
                EmployeeID = e.EmployeeID,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Role = e.Role
            }).ToList();
            return Ok(employeeDtos);
        }

        // GET: api/EmployeeAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeModel(int id)
        {
            var e = await _context.Employees.FindAsync(id);
            if (e == null)
            {
                return NotFound();
            }
            var dto = new EmployeeDto
            {
                EmployeeID = e.EmployeeID,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Role = e.Role
            };
            return Ok(dto);
        }

        // PUT: api/EmployeeAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeModel(int id, UpdateEmployeeDto dto)
        {
            var employeeModel = await _context.Employees.FindAsync(id);
            if (employeeModel == null)
            {
                return NotFound();
            }
            employeeModel.FirstName = dto.FirstName;
            employeeModel.LastName = dto.LastName;
            employeeModel.Email = dto.Email;
            employeeModel.Role = dto.Role;
            employeeModel.HourlyRate = dto.HourlyRate;
            employeeModel.DateHired = dto.DateHired;
            _context.Entry(employeeModel).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/EmployeeAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> PostEmployeeModel(CreateEmployeeDto dto)
        {
            var employeeModel = new EmployeeModel
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Role = dto.Role,
                HourlyRate = dto.HourlyRate,
                DateHired = dto.DateHired
            };
            _context.Employees.Add(employeeModel);
            await _context.SaveChangesAsync();
            var resultDto = new EmployeeDto
            {
                EmployeeID = employeeModel.EmployeeID,
                FirstName = employeeModel.FirstName,
                LastName = employeeModel.LastName,
                Email = employeeModel.Email,
                Role = employeeModel.Role
            };
            return CreatedAtAction("GetEmployeeModel", new { id = employeeModel.EmployeeID }, resultDto);
        }

        // DELETE: api/EmployeeAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeModel(int id)
        {
            var employeeModel = await _context.Employees.FindAsync(id);
            if (employeeModel == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employeeModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeModelExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeID == id);
        }
    }
} 
