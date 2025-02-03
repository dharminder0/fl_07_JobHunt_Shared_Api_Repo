using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VendersCloud.Business.Service.Abstract;
using static VendersCloud.Data.Enum.Enum;

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
        [Route("api/V1/UserProfiles/Insert")]

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
        [HttpGet]
        [Route("api/V1/UserProfiles/GetProfileRole")]
        public async Task<IActionResult> GetProfileRole(int userId)
        {
            try
            {
                var result = await _userProfilesService.GetProfileRole(userId);
               if(result!=null)
                {
                    string roleName = Enum.GetName(typeof(RoleType), result);
                    return Json(roleName);
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
