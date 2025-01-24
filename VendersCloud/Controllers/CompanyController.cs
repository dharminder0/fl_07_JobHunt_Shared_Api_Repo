using Microsoft.AspNetCore.Mvc;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Service.Abstract;

namespace VendersCloud.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : BaseApiController
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }


        [HttpGet]
        [Route("CompanyDetails/{companyCode}")]
        ///<summary>
        ///fetch company details by companyCode
        ///<param name="companyCode"></param>
        ///</summary>
        public async Task<IActionResult> GetCompanyDetailByCompanyCodeAsync(string companyCode)
        {
            try
            {
                var result = await _companyService.GetCompanyDetailByCompanyCodeAsync(companyCode);
                return Json(result);
            }
            catch (Exception ex) {
            return BadRequest(ex.Message);
            
            }
        }

        [HttpPost]
        [Route("UpsertAsync")]
        ///<summary>
        ///upsert
        ///<param name ="
        /// </summary>
        public async Task<IActionResult> UpsertAsync(string companyName, string email, string companyCode)
        {
            try
            {
                var result = await _companyService.UpsertAsync(companyName, email, companyCode);
                return Json(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CompanyInfo")]
        ///<summary>
        ///Company Information
        ///<param name="companyInfo"></param>
        /// </summary>

        public async Task<IActionResult> AddCompanyInformationAsync(CompanyInfoRequestModel companyInfo)
        {
            try
            {
                var result = await _companyService.AddCompanyInformationAsync(companyInfo);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetCompanyList")]
        public async Task<IActionResult> GetAllCompanyDetailsAsync(string companyCode="", List<string> roleType=null)
        {
            try
            {
                var result = await _companyService.GetAllCompanyDetailsAsync(companyCode,roleType);

                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet]
        [Route("Company/UserList/{companyCode}")]
        ///<summary>
        ///Fetching All The User's List And Company
        /// By companyCode
        /// <param name="companyCode"></param>
        ///</summary>
        public async Task<IActionResult>GetCompanyUserListAsync(string companyCode)
        {
            try
            {
                var result = await _companyService.GetCompanyUserListAsync(companyCode);
                return Json(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
