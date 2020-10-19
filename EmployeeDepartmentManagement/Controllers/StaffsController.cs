using BusinessTier.Requests.StaffRequest;
using BusinessTier.Responses;
using BusinessTier.Services;
using BusinessTier.Utilities;
using BusinessTier.ViewModels;
using DataTier.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeDepartmentManagement.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffsController(IStaffService staffService)
        {
            _staffService = staffService;
        }


        //[ApiExplorerSettings(IgnoreApi = true)]
        // GET: api/Staffs
        [HttpGet]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME+","+Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<List<StaffViewModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<List<StaffViewModel>>>> GetStaffs()
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var roles = IdentityManager.GetRolesFromToken(raw);

            var staffs = _staffService.GetStaffs(roles);

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

        [HttpGet("department/{departmentId}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<List<StaffViewModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<List<StaffViewModel>>>> GetStaffsOfDepartment(string departmentId)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var roles = IdentityManager.GetRolesFromToken(raw);

            var staffs = _staffService.GetStaffsOfDepartment(departmentId, roles);

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

        // GET: api/Staffs/5
        [HttpGet("{staffId}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<StaffViewModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<StaffViewModel>>> GetDepartmentStaff(Guid staffId)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var roles = IdentityManager.GetRolesFromToken(raw);

            var staff = _staffService.GetStaffById(staffId, roles);

            if (staff == null)
            {
                return NotFound();
            }

            var result = new BaseResponse<StaffViewModel>()
            {
                Data = staff
            };

            return Ok(result);
        }

        // PUT: api/Staffs/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<BaseResponse<StaffViewModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<StaffViewModel>>> PutDepartmentStaff([FromRoute]Guid id,[FromBody] UpdateStaffRequest request)
        {
            try
            {
                var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
                var requester = IdentityManager.GetUsernameFromToken(raw);
                var roles = IdentityManager.GetRolesFromToken(raw);

                var staff = _staffService.UpdateStaff(id,request, requester,roles);

                if (staff == null)
                    return NotFound();

                return Ok(new BaseResponse<StaffViewModel>() { Data = staff });
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("ERR"))
                    return BadRequest(new ErrorResponse(ex.Message));
                else
                    throw;
            }
        }

        // POST: api/Staffs
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<StaffViewModel>), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<StaffViewModel>>> PostAccount([FromBody] CreateStaffRequest request)
        {
            try
            {
                var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
                var requester = IdentityManager.GetUsernameFromToken(raw);
                var roles = IdentityManager.GetRolesFromToken(raw);

                var result = _staffService.CreateStaff(request, requester, roles);

                if (result == null)
                {
                    return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest.ToString()
                        ,"Failed to create a staff") );
                }

                return Created(result.Id.ToString(), new BaseResponse<StaffViewModel>() { Data = result });
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("ERR"))
                    return BadRequest(new ErrorResponse(ex.Message));
                else
                    throw;
            }
        }

        // DELETE: api/Staffs/5
        [HttpDelete("{id}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<string>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<string>>> DeleteDepartmentStaff(Guid id)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var requester = IdentityManager.GetUsernameFromToken(raw);
            var roles = IdentityManager.GetRolesFromToken(raw);

            var staffId = _staffService.DeleteStaff(id, requester, roles);

            if (staffId == null)
            {
                return NotFound();
            }

            var result = new BaseResponse<string>()
            {
                Data = staffId,
                Message = "Removed staff successfully"
            };

            return result;
        }
    }
}
