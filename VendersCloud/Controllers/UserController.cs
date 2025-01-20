using Microsoft.AspNetCore.Mvc;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Service.Abstract;

namespace VendersCloud.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        ///<summary>
        ///Retrieves a information of user
        /// </summary>

        [HttpGet]
        [Route("getAllUsers")]
        public async Task<IActionResult> GetAllUsersInfoAsync()
        {
            try
            {
                var result = await _userService.GetAllUsersInfoAsync();
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        ///<summary>
        ///User Login 
        /// <param name="userLogin"></param>
        ///</summary>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> UserLoginAsync(UserLoginRequestModel userLogin)
        {
            try
            {
                var result = await _userService.UserLoginAsync(userLogin);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        ///<summary>
        ///User SignUp
        ///<param name="usersign"></param>
        ///</summary>
        [HttpPost]
        [Route("signUp")]
        public async Task<IActionResult> UserSignUpAsync(UserSignUpRequestModel usersign)
        {
            try
            {
                var result = await _userService.UserSignUpAsync(usersign);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
