using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTier.Models;
using DataTier.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeDepartmentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUnitOfWork _uow;
        public UserController(IUnitOfWork uow){
            _uow = uow;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _uow.Repository<User>().GetAll();
    }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
