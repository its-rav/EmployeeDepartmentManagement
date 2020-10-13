using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataTier.Models;
using BusinessTier.Requests;
using BusinessTier.Services;
using BusinessTier.ViewModels;
using BusinessTier.Requests.UserRequest;
using BusinessTier.Responses;
using BusinessTier.Utilities;
using Microsoft.AspNetCore.Authorization;
using AutoMapper.Internal;

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

        // GET: api/Users
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<UserViewModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<List<UserViewModel>>>> GetAccounts()
        {
            var accounts = _userService.GetUsers();

            if (accounts == null)
            {
                return NotFound();
            }
            var result = new BaseResponse<List<UserViewModel>>
            {
                Data = accounts
            };
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        // GET: api/users/5
        [HttpGet("{username}")]
        public async Task<ActionResult<Account>> GetAccount(string username)
        {
            var account = _userService.FindUserById(username);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(BaseResponse<dynamic>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<dynamic>>> AuthenticateAccount([FromBody] AuthenticateRequest req)
        {
            var (token,user)=_userService.Authenticate(req.Username, req.Password);

            if (token==null)
            {
                return NotFound();
            }

            var result = new BaseResponse<dynamic>
            {
                Data = new {
                    Token=token,
                    User=user 
                }
            };

            return Ok(result);
        }

        // PUT: api/users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(Guid id, Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //
            }

            return NoContent();
        }

        // POST: api/users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize(Roles = Constants.ROLE_ADMIN_NAME)]
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<UserViewModel>), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<BaseResponse<UserViewModel>>> PostAccount([FromBody]CreateAccountRequest request)
        {
            try
            {
                var raw = Request.Headers.FirstOrDefault(x=>x.Key.Equals("Authorization")).Value;
                var requester = IdentityManager.GetUsernameFromToken(raw);


                var result = _userService.CreateUser(request, requester);
                return Created(result.Id.ToString(),new BaseResponse<UserViewModel>() { Data = result});
            }
            catch(Exception ex)
            {
                if (ex.Message.StartsWith("ERR"))
                    return BadRequest(new ErrorResponse(ex.Message));
                else
                    throw;
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Account>> DeleteAccount(Guid id)
        {
            var account = await _context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Account.Remove(account);
            await _context.SaveChangesAsync();

            return account;
        }
    }
}
