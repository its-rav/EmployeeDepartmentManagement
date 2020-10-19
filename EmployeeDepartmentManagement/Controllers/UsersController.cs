using BusinessTier.Requests;
using BusinessTier.Requests.UserRequest;
using BusinessTier.Responses;
using BusinessTier.Services;
using BusinessTier.Utilities;
using BusinessTier.ViewModels;
using DataTier.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class UsersController : ControllerBase
    {
        private readonly EDMContext _context;
        private readonly IUserService _userService;
        public UsersController(IUserService userService, EDMContext context)
        {
            _userService = userService;
            _context = context;
        }

        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(BaseResponse<dynamic>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<dynamic>>> AuthenticateAccount([FromBody] AuthenticateRequest req)
        {
            var (token, user) = _userService.Authenticate(req.Username, req.Password);

            if (token == null)
            {
                return NotFound();
            }

            var result = new BaseResponse<dynamic>
            {
                Data = new
                {
                    Token = token,
                    User = user
                }
            };

            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(BaseResponse<UserViewModel>), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<UserViewModel>>> GetUser()
        {
            var raw = Request.Headers.FirstOrDefault(x => x.Key.Equals("Authorization")).Value;
            var userId = IdentityManager.GetUserIdFromToken(raw);

            var user = _userService.FindUserById(new Guid(userId));

            if (user == null)
            {
                return NotFound();
            }

            var result = new BaseResponse<UserViewModel>
            {
                Data = user
            };

            return Ok(result);
        }

    }
}
