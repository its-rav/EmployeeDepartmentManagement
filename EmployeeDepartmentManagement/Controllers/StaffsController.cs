using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataTier.Models;
using BusinessTier.Services;
using BusinessTier.ViewModels;
using BusinessTier.Responses;

namespace EmployeeDepartmentManagement.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly EDMContext _context;

        private readonly IStaffService _staffService;

        public StaffsController(IStaffService staffService, EDMContext context)
        {
            _staffService = staffService;
            _context = context;
        }


        //[ApiExplorerSettings(IgnoreApi = true)]
        // GET: api/Staffs
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<StaffViewModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<List<StaffViewModel>>>> GetStaffs()
        {
            var staffs = _staffService.GetStaffs();

            if (staffs == null)
            {
                return NotFound();
            }


            var result = new BaseResponse<List<StaffViewModel>>()
            {
                Data = staffs
            };

            return Ok(result);
        }

        [HttpGet("department/{id}")]
        public async Task<ActionResult<IEnumerable<DepartmentStaff>>> GetStaffsOfDepartment(string id)
        {
            return await _context.DepartmentStaff.Where(x=>x.DepartmentId==id).ToListAsync();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
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

        [ApiExplorerSettings(IgnoreApi = true)]
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
        [ApiExplorerSettings(IgnoreApi = true)]
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

        [ApiExplorerSettings(IgnoreApi = true)]
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
