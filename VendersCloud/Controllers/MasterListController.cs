namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class MasterListController : BaseApiController
    {
        private readonly IMasterListService _masterListService;
        public MasterListController(IMasterListService masterListService)
        {
            _masterListService= masterListService;
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet]
        [Route("api/v1/MasterList/List")]
        public async Task<IActionResult> GetMasterListAsync()
        {
            try
            {
                var result = await _masterListService.GetMasterListAsync();
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/v1/MasterList/BulkAdd")]

        public async Task<IActionResult>AddBulkMasterListAsync(List<string> name)
        {
            try
            {
                var result = await _masterListService.AddBulkMasterListAsync(name);
                return Json(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet]
        [Route("api/v1/MasterList/{name}")]
        public async Task<IActionResult> GetMasterListByIdAndNameAsync(string name )
        {
            try
            {
                var result = await _masterListService.GetMasterListByIdAndNameAsync(name);
                return Json(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}
