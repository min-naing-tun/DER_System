using DER_System.Repository;
using DER_System.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DER_System.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        public readonly MaterialRepository _repo;
        public MaterialController(MaterialRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMaterial()
        {
            DataTable dt = await _repo.GetAllAsync();
            return Ok(dt);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MaterialModel model)
        {
            ResponseModel r = await _repo.CreateAsync(model);
            return Ok(r);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MaterialModel model, string key)
        {
            ResponseModel r = await _repo.UpdateAsync(model, key);
            return Ok(r);
        }

        [HttpPost]
        public async Task<IActionResult> Persist([FromBody] MaterialModel model)
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
