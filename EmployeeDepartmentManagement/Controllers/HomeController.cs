using Microsoft.AspNetCore.Mvc;

namespace EmployeeDepartmentManagement.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Ok");
        }
    }
}
