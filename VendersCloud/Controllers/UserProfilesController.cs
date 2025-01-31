using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VendersCloud.Business.Service.Abstract;

namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class UserProfilesController : BaseApiController
    {
        private readonly IUserProfilesService _userProfilesService;
        public UserProfilesController(IUserProfilesService userProfilesService)
        {
            _userProfilesService = userProfilesService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/V1/UserProfiles/Upsert")]

        public async Task<IActionResult> UpsertUserProfileAsync(int userId, int profileId)
        {
            try
            {
                var result = await _userProfilesService.UpsertUserProfileAsync(userId, profileId);
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
        [Route("api/V1/UserProfiles/GetProfileRole")]
        public async Task<IActionResult> GetProfileRole(int userId)
        {
            try
            {
                var result = await _userProfilesService.GetProfileRole(userId);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
