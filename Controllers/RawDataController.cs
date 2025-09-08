using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DER_System.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RawDataController : ControllerBase
    {
        public RawDataController()
        {

        }

        [HttpGet]
        public IActionResult GetAll()
        {
            DataTable dt = new DataTable();
            return Ok(dt);
        }
    }
}
