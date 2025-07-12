namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class ListValuesController : BaseApiController
    {
        private readonly IListValuesService _listValuesService;
        public ListValuesController(IListValuesService listValuesService)
        {
            _listValuesService = listValuesService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpGet]
        [Route("api/v1/ListValues/List")]
        public async Task<IActionResult> GetListValuesAsync()
        {
            try
            {
                var result = await _listValuesService.GetListValuesAsync();
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpGet]
        [Route("api/v1/ListValues/Name")]
        public async Task<IActionResult> GetListValuesByNameAsync(string Name)
        {
            try
            {
                var result = await _listValuesService.GetListValuesByNameAsync(Name);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpGet]
        [Route("api/v1/ListValues/MasterListValue")]
        public async Task<IActionResult> GetListValuesByIdAsync(string name)
        {
            try
            {
                var result = await _listValuesService.GetListValuesByMasterListIdAsync(name);
                return Json(result);
            }
            catch (Exception ex)
            {
               return BadRequest(ex.Message);
            }
        }
    }
}
