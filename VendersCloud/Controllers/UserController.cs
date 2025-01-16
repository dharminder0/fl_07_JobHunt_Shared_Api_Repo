﻿using Microsoft.AspNetCore.Mvc;
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
        ///<summary>
        ///User Login 
        /// <param name="userLogin"></param>
        ///</summary>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> UserLogin(UserLoginRequestModel userLogin)
        {
            try
            {
                var result = await _userService.UserLogin(userLogin);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        ///<summary>
        ///User SignUp
        ///<param name="companyName"></param>
        ///<param name="email"></param>
        ///<param name="password"></param>
        ///</summary>
        [HttpPost]
        [Route("signUp")]
        public async Task<IActionResult> UserSignUp(string companyName, string email, string password)
        {
            try
            {
                var result = await _userService.UserSignUp(companyName, email, password);
                return Json("UserId:- "+result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
