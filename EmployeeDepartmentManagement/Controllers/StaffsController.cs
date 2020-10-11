using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataTier.Models;

namespace EmployeeDepartmentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly EDMContext _context;

        public StaffsController(EDMContext context)
        {
            _context = context;
        }

        // GET: api/Staffs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentStaff>>> GetDepartmentStaff()
        {
            return await _context.DepartmentStaff.ToListAsync();
        }
        [HttpGet("department/{id}")]
        public async Task<ActionResult<IEnumerable<DepartmentStaff>>> GetStaffsOfDepartment(string id)
        {
            return await _context.DepartmentStaff.Where(x=>x.DepartmentId==id).ToListAsync();
        }
        // GET: api/Staffs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentStaff>> GetDepartmentStaff(Guid id)
        {
            var departmentStaff = await _context.DepartmentStaff.FindAsync(id);

            if (departmentStaff == null)
            {
                return NotFound();
            }

            return departmentStaff;
        }

        // PUT: api/Staffs/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartmentStaff(Guid id, DepartmentStaff departmentStaff)
        {
            if (id != departmentStaff.AccountId)
            {
                return BadRequest();
            }

            _context.Entry(departmentStaff).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentStaffExists(id))
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

        // POST: api/Staffs
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<DepartmentStaff>> PostDepartmentStaff(DepartmentStaff departmentStaff)
        {
            _context.DepartmentStaff.Add(departmentStaff);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DepartmentStaffExists(departmentStaff.AccountId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDepartmentStaff", new { id = departmentStaff.AccountId }, departmentStaff);
        }

        // DELETE: api/Staffs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DepartmentStaff>> DeleteDepartmentStaff(Guid id)
        {
            var departmentStaff = await _context.DepartmentStaff.FindAsync(id);
            if (departmentStaff == null)
            {
                return NotFound();
            }

            _context.DepartmentStaff.Remove(departmentStaff);
            await _context.SaveChangesAsync();

            return departmentStaff;
        }

        private bool DepartmentStaffExists(Guid id)
        {
            return _context.DepartmentStaff.Any(e => e.AccountId == id);
        }
    }
}
