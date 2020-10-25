using BusinessTier.Requests.StaffRequest;
using BusinessTier.Responses;
using BusinessTier.Services;
using BusinessTier.Utilities;
using static BusinessTier.Utilities.LinqUtils;
using BusinessTier.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        #region GET staffs
        // GET: api/Staffs
        /// <summary>
        /// Get staffs
        /// </summary>
        /// <param name="Q" in="query">Search keyword(by staff username)</param>
        /// <param name="Page" in="query">Page index</param>
        /// <param name="Size" in="query">Page size</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME+","+Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<List<StaffViewModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<List<StaffViewModel>>>> GetStaffs([FromQuery]string Q = "", int Page = 1, int Size = Constants.DEFAULT_PAGE_SIZE)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var roles = IdentityManager.GetRolesFromToken(raw);

            List<StaffViewModel> staffs = null;

            if (Q.Trim().Equals("")) { 
                staffs = _staffService.GetStaffs(Page, Size, roles);
            }
            else
            {
                staffs = _staffService.SearchStaffsByUsername(Q,Page, Size, roles);
            }

            if (staffs == null)
            {
                return NotFound();
            }

            var result = new BaseResponse<List<StaffViewModel>>()
            {
                Data = staffs,
                Page = Page,
                Size = Size
            };

            return Ok(result);
        }
        #endregion

        #region GET staffs/department/{departmentId}
        // GET: api/department/{departmentId}
        /// <summary>
        /// Get staffs in a department by department ID
        /// </summary>
        /// <param name="id">Deparment ID</param>
        /// <param name="Q" in="query">Search keyword(by staff username)</param>
        /// <param name="Page" in="query">Page index</param>
        /// <param name="Size" in="query">Page size</param>
        /// <returns></returns>
        [HttpGet("department/{id}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<List<StaffViewModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<List<StaffViewModel>>>> GetStaffsOfDepartment([FromRoute] string id, [FromQuery] string Q = "", int Page=1,int Size=Constants.DEFAULT_PAGE_SIZE)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var roles = IdentityManager.GetRolesFromToken(raw);

            List<StaffViewModel> staffs = null;

            if (Q.Trim().Equals(""))
            {
                staffs = _staffService.GetStaffsOfDepartment(id, Page, Size, roles);
            }
            else
            {
                staffs = _staffService.SearchStaffsOfDepartmentByUserName(id, Q, Page, Size, roles);
            }

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
        #endregion

        #region GET staffs/{id}
        // GET: api/Staffs/5
        /// <summary>
        /// Get a staff by ID
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<StaffViewModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<StaffViewModel>>> GetDepartmentStaff([FromRoute]Guid id)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var roles = IdentityManager.GetRolesFromToken(raw);

            var staff = _staffService.GetStaffById(id, roles);

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
        #endregion

        #region PUT: staffs/{id}
        // PUT: api/Staffs/5
        /// <summary>
        /// Update staff by ID
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <returns></returns>
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
        #endregion

        #region PUT staffs/{staffId}/staff/{departmentId}
        // PUT: api/Staffs/{staffId}/staff/{departmentId}
        /// <summary>
        /// Add a staff to a department
        /// </summary>
        /// <param name="staffId">Staff ID</param>
        /// <param name="departmentId">Department ID</param>
        /// <returns></returns>
        [HttpPut("{staffId}/staff/{departmentId}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> AddDepartmentToStaff([FromRoute] string departmentId, Guid staffId)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var requester = IdentityManager.GetUserIdFromToken(raw);
            var roles = IdentityManager.GetRolesFromToken(raw);

            try
            {
                _staffService.AddStaffToDepartment(staffId, departmentId, requester, roles);

                return Ok();
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("ERR"))
                    return BadRequest(new ErrorResponse(ex.Message));
                else
                    throw;
            }
        }
        #endregion
        #region POST staffs
        // POST: api/Staffs
        /// <summary>
        /// Create a staff
        /// </summary>
        /// <returns></returns>
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
        #endregion

        #region DELETE staff/{id}
        // DELETE: api/Staffs/5
        /// <summary>
        /// Remove staff by ID
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <returns></returns>
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
        #endregion
    }
}
