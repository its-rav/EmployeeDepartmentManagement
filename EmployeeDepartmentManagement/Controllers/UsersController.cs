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
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            var accounts = _userService.GetUsers();

            if (accounts == null)
            {
                return NotFound();
            }

            return Ok(accounts);
        }

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
        public async Task<ActionResult<Account>> AuthenticateAccount([FromBody] AuthenticateRequest req)
        {
            var result=_userService.Authenticate(req.Username, req.GetPasswordHash());

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // PUT: api/users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
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
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            try
            {
                var result = _userService.CreateUser(account);
                return Created(result.Id.ToString(), result);
            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("dup"))
                    return BadRequest("Existed user");
                else
                    return StatusCode(500);
            }
        }

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
