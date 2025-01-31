using Microsoft.AspNetCore.Mvc;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Service.Abstract;

namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class UsersController : BaseApiController
    {
        private readonly IUsersService _userService;
        public UsersController(IUsersService userService)
        {
            _userService = userService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/V1/users/signUp")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegistrationRequest request)
        {
            try
            {
                var result= await _userService.RegisterNewUserAsync(request);
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
        [HttpPost]
        [Route("api/V1/users/login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _userService.LoginUserAsync(request);
                return Json(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
