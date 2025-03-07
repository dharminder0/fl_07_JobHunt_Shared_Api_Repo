namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class OrgSocialController : BaseApiController
    {
        private readonly IOrgSocialService _orgSocialService;
        public OrgSocialController(IOrgSocialService orgSocialService)
        {
            _orgSocialService= orgSocialService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/v1/orgSocial/UpsertProfile")]

        public async Task<IActionResult> UpsertSocialProfile(OrgSocial social)
        {
            try
            {
                var result= await _orgSocialService.UpsertSocialProfile(social);
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
        [Route("api/v1/orgSocial/GetProfile")]
        public async Task<IActionResult> GetOrgSocialProfile(string orgCode)
        {
            try { 
                var result = await _orgSocialService.GetOrgSocialProfile(orgCode);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
