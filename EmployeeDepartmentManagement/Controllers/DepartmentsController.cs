using BusinessTier.Requests.DepartmentRequest;
using BusinessTier.Responses;
using BusinessTier.Services;
using BusinessTier.Utilities;
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
    public class DepartmentsController : ControllerBase
    {

        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }
        #region GET departments
        // GET: api/Departments
        /// <summary>
        /// Get departments
        /// </summary>
        /// <param name="Q">Search keyword(by department name)</param>
        /// <param name="Page">Page index</param>
        /// <param name="Size">Page size</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<List<DepartmentViewModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<List<DepartmentViewModel>>>> GetDepartments([FromQuery] string Q="", int Page = 1, int Size = Constants.DEFAULT_PAGE_SIZE)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var roles = IdentityManager.GetRolesFromToken(raw);

            List<DepartmentViewModel> departments = null;

            if (Q.Trim().Equals(""))
            {
                departments = _departmentService.GetDepartments(Page, Size, roles);
            }
            else
            {
                departments = _departmentService.SearchDepartmentsByName(Q,Page,Size,roles);
            }

            if (departments == null)
            {
                return NotFound();
            }

            var result = new BaseResponse<List<DepartmentViewModel>>()
            {
                Data = departments,
                Page = Page,
                Size = Size
            };

            return Ok(result);
        }
        #endregion

        #region GET deparments/staff/{id}
        /// <summary>
        /// Get departments of a staff by ID
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <param name="Q">Search keyword (by department name)</param>
        /// <param name="Page">Page index</param>
        /// <param name="Size">Page size</param>
        /// <returns></returns>
        [HttpGet("staff/{id}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<List<DepartmentViewModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<List<DepartmentViewModel>>>> GetDepartmentStaff(Guid id,[FromQuery] string Q = "", int Page = 1, int Size = Constants.DEFAULT_PAGE_SIZE)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var roles = IdentityManager.GetRolesFromToken(raw);

            List<DepartmentViewModel> departments = null;

            if (Q.Trim().Equals(""))
            {
                departments = _departmentService.GetDepartmentsOfStaff(id,Page, Size, roles);
            }
            else
            {
                departments = _departmentService.SearchDepartmentsOfStaffByName(id,Q, Page, Size, roles);
            }

            if (departments == null)
            {
                return NotFound();
            }

            var result = new BaseResponse<List<DepartmentViewModel>>()
            {
                Data = departments,
                Page = Page,
                Size = Size
            };

            return Ok(result);
        }
        #endregion

        #region GET departments/{id}
        /// <summary>
        /// Get a department by ID
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns></returns>
        // GET: api/Departments/5
        [HttpGet("{id}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<DepartmentViewModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<DepartmentViewModel>>> GetDepartment(string id)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var roles = IdentityManager.GetRolesFromToken(raw);

            var department = _departmentService.GetDepartmentById(id, roles);

            if (department == null)
            {
                return NotFound();
            }

            var result = new BaseResponse<DepartmentViewModel>()
            {
                Data = department
            };

            return Ok(result);
        }
        #endregion

        #region PUT departments/{id}
        /// <summary>
        /// Update a departments by ID
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns></returns>
        // PUT: api/Departments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<DepartmentViewModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<DepartmentViewModel>>> PutDepartment([FromRoute] string id, [FromBody] UpdateDepartmentRequest request)
        {
            try
            {
                var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
                var requester = IdentityManager.GetUsernameFromToken(raw);
                var roles = IdentityManager.GetRolesFromToken(raw);

                var department = _departmentService.UpdateDepartment(id, request, requester, roles);

                if (department == null)
                    return NotFound();

                return Ok(new BaseResponse<DepartmentViewModel>() { Data = department });
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

        #region PUT departments/{departmentId}/staff/{staffId}
        // PUT: api/Departments/{departmentId}/staff/{staffId}
        /// <summary>
        /// Add a department for a staff
        /// </summary>
        /// <param name="departmentId">Department ID</param>
        /// <param name="staffId">Staff ID</param>
        /// <returns></returns>
        [HttpPut("{departmentId}/staff/{staffId}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> AddStaffToDeparment([FromRoute]string departmentId,Guid staffId)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var requester = IdentityManager.GetUserIdFromToken(raw);
            var roles = IdentityManager.GetRolesFromToken(raw);

            try
            {
                _departmentService.AddStaffToDepartment(departmentId, staffId, requester, roles);

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

        #region POST departments
        /// <summary>
        /// Create a department
        /// </summary>
        /// <returns></returns>
        // POST: api/Departments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<DepartmentViewModel>), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<DepartmentViewModel>>> PostAccount([FromBody] CreateDepartmentRequest request)
        {
            try
            {
                var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
                var requester = IdentityManager.GetUsernameFromToken(raw);
                var roles = IdentityManager.GetRolesFromToken(raw);

                var result = _departmentService.CreateDepartment(request, requester, roles);

                if (result == null)
                {
                    return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest.ToString()
                        , "Failed to create a department"));
                }

                return Created(result.Id.ToString(), new BaseResponse<DepartmentViewModel>() { Data = result });
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

        #region DELETE departments/{id}
        // DELETE: api/Departments/5
        /// <summary>
        /// Remove a department by ID
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<string>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<string>>> DeleteDepartmentStaff(string id)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var requester = IdentityManager.GetUsernameFromToken(raw);
            var roles = IdentityManager.GetRolesFromToken(raw);

            var departmentId = _departmentService.DeleteDepartment(id, requester, roles);

            if (departmentId == null)
            {
                return NotFound();
            }

            var result = new BaseResponse<string>()
            {
                Data = departmentId,
                Message = "Removed department successfully"
            };

            return result;
        }
        #endregion
    }
}
