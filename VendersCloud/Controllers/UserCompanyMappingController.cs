using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VendersCloud.Business.Service.Abstract;

namespace VendersCloud.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCompanyMappingController : BaseApiController
    {
        private readonly IUserCompanyMappingService _userCompanyMappingService;
        public UserCompanyMappingController(IUserCompanyMappingService userCompanyMappingService)
        {
            _userCompanyMappingService = userCompanyMappingService;
        }
        [HttpGet]
        [Route("GetMapping/{userId}")]
        ///<summary>
        ///GetMappingsByUserIdAsync
        ///<param name ="userId"></param>
        /// </summary>
        public async Task<IActionResult> GetMappingsByUserIdAsync(string userId)
        {
            try
            {
                var result = await _userCompanyMappingService.GetMappingsByUserIdAsync(userId);
                return Ok(result);

            }
            catch (Exception ex) {
            return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Add")]
        ///<summary>
        ///Add User Company Mapping
        ///<param name="userId"></param>
        ///<param name="companyCode"></param>
        ///</summary>
        public async Task<IActionResult> AddMappingAsync(string userId, string companyCode)
        {
            try
            {
                var result = await _userCompanyMappingService.AddMappingAsync(userId, companyCode);
                return Json(result);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
