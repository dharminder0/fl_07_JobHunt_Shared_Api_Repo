namespace VendersCloud.WebApi.Controllers
{

    [ApiController]
    public class PromptController : BaseApiController
    {
        private readonly IPromptService  _promptService;
        private IConfiguration _configuration;
        public PromptController(IPromptService promptService, IConfiguration configuration)
        {
            _promptService = promptService;
            _configuration = configuration;
                
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/V1/Prompt/GenerateContent")]

        public async Task<IActionResult> GenerateContent(PromptRequest promptRequest)
        {
            try
            {
                var result = await _promptService.GenerateUpdatedContent(promptRequest);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
