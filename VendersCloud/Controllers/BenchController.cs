using Microsoft.AspNetCore.Mvc;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Service.Abstract;

namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class BenchController : BaseApiController
    {
        private readonly IBenchService _benchService;
        public BenchController(IBenchService benchService) {
        _benchService= benchService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/V1/Bench/Upsert")]

        public async Task<IActionResult> UpsertBenchAsync(BenchRequest request)
        {
            try
            {
                var result = await _benchService.UpsertBenchAsync(request);
                return Json(result);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet]
        [Route("api/V1/Bench/{orgCode}")]

        public async Task<IActionResult> GetBenchListAsync(string orgCode)
        {
            try
            {
                var result = await _benchService.GetBenchListAsync(orgCode);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
