namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class OrganizationController : BaseApiController
    {
        private readonly IOrganizationService _organizationService;
        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/V1/Organization/GetOrganization")]
        public async Task<IActionResult> GetOrganizationDataAsync(string orgCode)
        {
            try
            {
                var result = await _organizationService.GetOrganizationDataAsync(orgCode);
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
        [Route("api/V1/Organization/List")]
        public async Task<IActionResult> GetOrganizationListAsync()
        {
            try
            {
                var result = await _organizationService.GetOrganizationListAsync();
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
        [Route("api/V1/Organization/AddInfo")]
        public async Task<IActionResult> AddOrganizationInfo(CompanyInfoRequest infoRequest)
        {
            try
            {
                var result = await _organizationService.AddOrganizationInfoAsync(infoRequest);
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
        [Route("api/V1/Organization/UpsertProfile")]
        public async Task<IActionResult> UpsertOrganizationProfile(OrganizationProfileRequest request)
        {
            try
            {
                var result = await _organizationService.UpsertOrganizationProfile(request);
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
        [HttpGet]
        [Route("api/V1/Organization/GetProfile")]
         public async Task<IActionResult> GetOrganizationProfile(string orgCode)
        {
            try
            {
                var result= await _organizationService.GetOrganizationProfile(orgCode);
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
        [HttpPost]
        [Route("api/V1/Organization/DispatchedInvitation")]
        public async Task<IActionResult> DispatchedOrganizationInvitationAsync(DispatchedInvitationRequest request )
        {
            try
            {
                var result = await _organizationService.DispatchedOrganizationInvitationAsync(request);
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
        [Route("api/V1/Organization/manageInvitation")]
        public async Task<IActionResult> ManageRelationshipStatusAsync(int id, int status)
        {
            try
            {
                var result= await _organizationService.ManageRelationshipStatusAsync(id, status);
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
        [Route("api/V1/Organization/List/Invitation")]
        public async Task<IActionResult> GetListRelationshipAsync(OrgRelationshipSearchRequest request)
        {
            try
            {
                var result = await _organizationService.GetListRelationshipAsync(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
