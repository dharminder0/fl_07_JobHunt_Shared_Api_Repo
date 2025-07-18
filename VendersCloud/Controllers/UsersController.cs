﻿namespace VendersCloud.WebApi.Controllers
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/users/delete")]
        public async Task<IActionResult> DeleteUserAsync(string Email, string OrganizationCode)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(Email, OrganizationCode);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpGet]
        [Route("api/V1/users/{email}")]

        public async Task<IActionResult> GetUserByEmailAsync(string email)
        {
            try
            {
                var result = await _userService.GetUserByEmailAsync(email);
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
        [Route("api/V1/all-users")]
        public async Task<IActionResult> GetAllUserAsync()
        {
            try
            {
                var result = await _userService.GetAllUserAsync();
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpGet]
        [Route("api/V1/{orgCode}/users")]

        public async Task<IActionResult> GetUserByOrgCodeAsync(string orgCode)
        {
            try
            {
                var result = await _userService.GetUserByOrgCodeAsync(orgCode);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/users/Profile/Add")]

        public async Task<IActionResult> InsertUserProfileAsync(int userId, int profileId)
        {
            try
            {
                var result= await _userService.InsertUserProfileAsync(userId, profileId);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/users/Email/Verify")]
        public async Task<IActionResult> VerifyUserEmailAsync(string userToken, string otp)
        {
            try
            {
                var result = await _userService.VerifyUserEmailAsync(userToken, otp);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/users/resend-email")]
        public async Task<IActionResult> ResendEmailAsync(string EmailId)
        {
            try
            {
                var result= await _userService.ResendEmailVerificationAsync(EmailId);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/users/UpdateProfile")]

        public async Task<IActionResult> UpdateUserProfileAsync(UpdateUserProfileRequest  request)
        {
            try
            {
                var result= await _userService.UpdateUserProfileAsync(request);
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
        [HttpPost]
        [Route("api/V1/users/ChangePassword")]

        public async Task<IActionResult> UpdateUserPasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var result = await _userService.UpdateUserPasswordAsync(request);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/user/AddMember")]
        public async Task<IActionResult> AddOrganizationMemberAsync(AddMemberRequest request)
        {
            try
            {
                var result = await _userService.AddOrganizationMemberAsync(request);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/users/SetPassword")]

        public async Task<IActionResult> SetPasswordAsync(SetPasswordRequest request)
        {
            try
            {
                var result = await _userService.SetPasswordAsync(request);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/users/ForgetPassword")]

        public async Task<IActionResult> ForgetPasswordAsync(string email)
        {
            try
            {
                var result = await _userService.ForgetPasswordAsync(email);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/users/member/Search")]

        public async Task<IActionResult> SearchMemberDetailsAsync(SearchMemberRequest request)
        {
            try
            {
                var result = await _userService.SearchMemberDetailsAsync(request);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/users/ChangeEmail")]
        public async Task<IActionResult> ChangeEmailAsync(string OldEmail, string NewEmail)
        {
            try
            {
                var result = await _userService.ChangeEmailAsync(OldEmail, NewEmail);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/users/DeleteMember")]
        public async Task<IActionResult> DeleteMemberByIdAsync(int userId)
        {
            try
            {
                var result = await _userService.DeleteMemberByIdAsync(userId);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
 