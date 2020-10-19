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

        // GET: api/Departments
        [HttpGet]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<List<DepartmentViewModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<List<DepartmentViewModel>>>> GetDepartments()
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var roles = IdentityManager.GetRolesFromToken(raw);

            var departments = _departmentService.GetDepartments(roles);

            if (departments == null)
            {
                return NotFound();
            }

            var result = new BaseResponse<List<DepartmentViewModel>>()
            {
                Data = departments
            };

            return Ok(result);
        }

        [HttpGet("staff/{staffId}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<List<DepartmentViewModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<List<DepartmentViewModel>>>> GetDepartmentStaff(Guid staffId)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var roles = IdentityManager.GetRolesFromToken(raw);

            var departments = _departmentService.GetDepartmentsOfStaff(staffId, roles);

            if (departments == null)
            {
                return NotFound();
            }

            var result = new BaseResponse<List<DepartmentViewModel>>()
            {
                Data = departments
            };

            return Ok(result);
        }

        // GET: api/Departments/5
        [HttpGet("{departmentId}")]
        [Authorize(Roles = (Constants.ROLE_ADMIN_NAME + "," + Constants.ROLE_MOD_NAME))]
        [ProducesResponseType(typeof(BaseResponse<DepartmentViewModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<DepartmentViewModel>>> GetDepartment(string departmentId)
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var roles = IdentityManager.GetRolesFromToken(raw);

            var department = _departmentService.GetDepartmentById(departmentId, roles);

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

        // DELETE: api/Departments/5
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
    }
}
