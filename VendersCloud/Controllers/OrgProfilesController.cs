using Microsoft.AspNetCore.Mvc;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Service.Abstract;

namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class OrgProfilesController : BaseApiController 
    {
        private readonly IOrgProfilesService _orgProfilesService;
        public OrgProfilesController(IOrgProfilesService orgProfilesService)
        {
            _orgProfilesService = orgProfilesService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/V1/OrgProfiles/Add")]

        public async Task<IActionResult> AddOrgProfileAsync(string orgCode, int profileId)
        {
            try
            {
                var result = await _orgProfilesService.AddOrganizationProfileAsync(orgCode, profileId);
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
        [Route("api/V1/OrgProfiles/Search")]
        public async Task<IActionResult> SearchOrganizationsDetails(SearchRequest request)
        {
            try
            {
                var result= await _orgProfilesService.SearchOrganizationsDetails(request);
                return Json(result);
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message);
            }
        }

    }
}
