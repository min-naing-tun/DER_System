using DER_System.Helper;
using DER_System.Repository;
using DER_System.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DER_System.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerMaterialListingController : ControllerBase
    {
        public readonly CustomerMaterialListingRepository _repo;
        public CustomerMaterialListingController(CustomerMaterialListingRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomerMaterial()
        {
            DataTable dt = await _repo.GetAllAsync();
            return Ok(dt);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerMaterialListingModel model)
        {
            ResponseModel r = await _repo.CreateAsync(model);
            return Ok(r);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CustomerMaterialListingModel model, string key)
        {
            ResponseModel r = await _repo.UpdateAsync(model, key);
            return Ok(r);
        }

        [HttpPost]
        public async Task<IActionResult> Persist([FromBody] CustomerMaterialListingModel model)
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
