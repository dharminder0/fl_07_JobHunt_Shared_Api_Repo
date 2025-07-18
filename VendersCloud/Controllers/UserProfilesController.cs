﻿namespace VendersCloud.WebApi.Controllers
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/UserProfiles/Add")]

        public async Task<IActionResult> InsertUserProfileAsync(int userId, int profileId)
        {
            try
            {
                var result = await _userProfilesService.InsertUserProfileAsync(userId, profileId);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpGet]
        [Route("api/V1/UserProfiles/GetProfileRole")]
        public async Task<IActionResult> GetProfileRole(int userId)
        {
            try
            {
                var results = await _userProfilesService.GetProfileRole(userId);
               
                return Json(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
