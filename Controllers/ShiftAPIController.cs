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
    public class ShiftAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShiftAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ShiftAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShiftModel>>> GetShifts()
        {
            return await _context.Shifts.ToListAsync();
        }

        // GET: api/ShiftAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShiftModel>> GetShiftModel(int id)
        {
            var shiftModel = await _context.Shifts.FindAsync(id);

            if (shiftModel == null)
            {
                return NotFound();
            }

            return shiftModel;
        }

        // PUT: api/ShiftAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShiftModel(int id, ShiftModel shiftModel)
        {
            if (id != shiftModel.ShiftID)
            {
                return BadRequest();
            }

            _context.Entry(shiftModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShiftModelExists(id))
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

        // POST: api/ShiftAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ShiftModel>> PostShiftModel(ShiftModel shiftModel)
        {
            _context.Shifts.Add(shiftModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShiftModel", new { id = shiftModel.ShiftID }, shiftModel);
        }

        // DELETE: api/ShiftAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShiftModel(int id)
        {
            var shiftModel = await _context.Shifts.FindAsync(id);
            if (shiftModel == null)
            {
                return NotFound();
            }

            _context.Shifts.Remove(shiftModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShiftModelExists(int id)
        {
            return _context.Shifts.Any(e => e.ShiftID == id);
        }
    }
}
