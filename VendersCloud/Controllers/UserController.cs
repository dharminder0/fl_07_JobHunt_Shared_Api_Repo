using Microsoft.AspNetCore.Mvc;
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
            _userService= userService;
        }

        ///<summary>
        ///Retrieves a information of user
        /// </summary>

        [HttpGet]
        [Route("getAllUsers")]
        public async Task<IActionResult> GetAllUsersInfo()
        {
            try
            {
                var result = await _userService.GetAllUsersInfo();
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
