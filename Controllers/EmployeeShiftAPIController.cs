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
    public class EmployeeShiftAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeShiftAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeShiftAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeShiftModel>>> GetEmployeeShiftModel()
        {
            return await _context.EmployeeShiftModel.ToListAsync();
        }

        // GET: api/EmployeeShiftAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeShiftModel>> GetEmployeeShiftModel(int id)
        {
            var employeeShiftModel = await _context.EmployeeShiftModel.FindAsync(id);

            if (employeeShiftModel == null)
            {
                return NotFound();
            }

            return employeeShiftModel;
        }

        // PUT: api/EmployeeShiftAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeShiftModel(int id, EmployeeShiftModel employeeShiftModel)
        {
            if (id != employeeShiftModel.EmployeeShiftID)
            {
                return BadRequest();
            }

            _context.Entry(employeeShiftModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeShiftModelExists(id))
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

        // POST: api/EmployeeShiftAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmployeeShiftModel>> PostEmployeeShiftModel(EmployeeShiftModel employeeShiftModel)
        {
            _context.EmployeeShiftModel.Add(employeeShiftModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployeeShiftModel", new { id = employeeShiftModel.EmployeeShiftID }, employeeShiftModel);
        }

        // DELETE: api/EmployeeShiftAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeShiftModel(int id)
        {
            var employeeShiftModel = await _context.EmployeeShiftModel.FindAsync(id);
            if (employeeShiftModel == null)
            {
                return NotFound();
            }

            _context.EmployeeShiftModel.Remove(employeeShiftModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/EmployeeShiftAPI/link
        [HttpPost("link")]
        public async Task<IActionResult> LinkEmployeeToShift([FromBody] LinkEmployeeShiftDto dto)
        {
            if (dto == null || dto.EmployeeID == 0 || dto.ShiftID == 0)
                return BadRequest("Invalid EmployeeID or ShiftID");

            var exists = await _context.EmployeeShiftModel.AnyAsync(es => es.EmployeeID == dto.EmployeeID && es.ShiftID == dto.ShiftID);
            if (exists)
                return Conflict("Employee is already linked to this shift.");

            var model = new EmployeeShiftModel { EmployeeID = dto.EmployeeID, ShiftID = dto.ShiftID };
            _context.EmployeeShiftModel.Add(model);
            await _context.SaveChangesAsync();
            return Ok(dto);
        }

        // DELETE: api/EmployeeShiftAPI/unlink?employeeId=1&shiftId=2
        [HttpDelete("unlink")]
        public async Task<IActionResult> UnlinkEmployeeFromShift([FromQuery] int employeeId, [FromQuery] int shiftId)
        {
            var link = await _context.EmployeeShiftModel.FirstOrDefaultAsync(es => es.EmployeeID == employeeId && es.ShiftID == shiftId);
            if (link == null)
                return NotFound("Link not found.");

            _context.EmployeeShiftModel.Remove(link);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/EmployeeShiftAPI/employees-in-shift/{shiftId}
        [HttpGet("employees-in-shift/{shiftId}")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesInShift(int shiftId)
        {
            var employees = await _context.EmployeeShiftModel
                .Where(es => es.ShiftID == shiftId)
                .Include(es => es.Employee)
                .Select(es => es.Employee)
                .ToListAsync();
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

        // GET: api/EmployeeShiftAPI/shifts-for-employee/{employeeId}
        [HttpGet("shifts-for-employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<ShiftDto>>> GetShiftsForEmployee(int employeeId)
        {
            var shifts = await _context.EmployeeShiftModel
                .Where(es => es.EmployeeID == employeeId)
                .Include(es => es.Shift)
                .Select(es => es.Shift)
                .ToListAsync();
            var shiftDtos = shifts.Select(s => new ShiftDto
            {
                ShiftID = s.ShiftID,
                StartDateTime = s.StartDateTime,
                EndDateTime = s.EndDateTime
            }).ToList();
            return Ok(shiftDtos);
        }

        private bool EmployeeShiftModelExists(int id)
        {
            return _context.EmployeeShiftModel.Any(e => e.EmployeeShiftID == id);
        }
    }
}
