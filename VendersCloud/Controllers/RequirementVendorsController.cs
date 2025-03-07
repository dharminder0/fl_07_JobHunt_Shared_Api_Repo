namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class RequirementVendorsController : BaseApiController
    {
        private readonly IRequirementVendorsService _requirementVendorsService;
        public RequirementVendorsController(IRequirementVendorsService requirementVendorsService)
        {
            _requirementVendorsService= requirementVendorsService;
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/V1/RequirementVendors/ShareRequirement")]

        public async Task<IActionResult> AddRequirementShareData(RequirementSharedRequest request)
        {
            try
            {
                var result= await _requirementVendorsService.AddRequirementShareData(request);
                return Json(result);
            }
            catch (Exception ex) {
             return BadRequest(ex.Message);
            }
        }
    }
}
