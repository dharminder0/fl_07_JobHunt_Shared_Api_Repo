﻿namespace VendersCloud.WebApi.Controllers
{

    [ApiController]
    public class PromptController : BaseApiController
    {
        private readonly IPromptService _promptService;

        public PromptController(IPromptService promptService)
        {
            _promptService = promptService;


        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
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


