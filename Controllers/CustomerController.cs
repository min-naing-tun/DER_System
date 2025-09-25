using DER_System.Repository;
using DER_System.Utilities;
using DER_System.ViewModels;
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
        public async Task<IActionResult> GetAllCustomer()
        {
            DataTable dt = await _repo.GetAllAsync();
            return Ok(dt);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerModel model)
        {
            ResponseModel r = await _repo.CreateAsync(model);
            return Ok(r);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CustomerModel model, string key)
        {
            ResponseModel r = await _repo.UpdateAsync(model, key);
            return Ok(r);
        }

        [HttpPost]
        public async Task<IActionResult> Persist([FromBody] CustomerModel model)
        {
            ResponseModel r = await _repo.PersistAsync(model);
            return Ok(r);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string key)
        {
            ResponseModel r = await _repo.DeleteAsync(key);
            return Ok(r);
        }
    }
}
