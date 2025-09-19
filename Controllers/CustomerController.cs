using DER_System.Repository;
using DER_System.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DER_System.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        public readonly CustomerRepository _repo;
        public CustomerController(CustomerRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetFirst10Customers()
        {
            DataTable dt = await _repo.GetAllAsync();
            return Ok(dt);
        }
    }
}
