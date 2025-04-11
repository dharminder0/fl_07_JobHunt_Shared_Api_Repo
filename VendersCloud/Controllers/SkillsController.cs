namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class SkillsController : BaseApiController
    {
        private readonly ISkillService _skillService;
        public SkillsController(ISkillService skillService) 
        {
            _skillService = skillService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/Skill/Upsert")]
        public async Task<IActionResult> SkillUpsertAsync(List<string> skillnames)
        {
            try
            {
                var result = await _skillService.SkillUpsertAsync(skillnames);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpGet]
        [Route("api/V1/Skill/GetList")]
        public async Task<IActionResult> GetSkillListAsync()
        {
            try
            {
                var result = await _skillService.GetSkillListAsync();
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
