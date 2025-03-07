namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class ClientsController : BaseApiController
    {
        private readonly IClientsService _clientsService;
        public ClientsController(IClientsService clientsService)
        {
            _clientsService= clientsService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/V1/Clients/Upsert")]

        public async Task<IActionResult> UpsertClientAsync(ClientsRequest request)
        {
            try
            {
                var result = await _clientsService.UpsertClientAsync(request);
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
        [HttpGet]
        [Route("api/V1/Clients/Id")]
        public async Task<IActionResult> GetClientsByIdAsync(int id)
        {
            try
            {
                var results = await _clientsService.GetClientsByIdAsync(id);

                return Json(results);
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
        [HttpGet]
        [Route("api/V1/Clients/ClientName")]
        public async Task<IActionResult> GetClientsByNameAsync(string name)
        {
            try
            {
                var results = await _clientsService.GetClientsByNameAsync(name);

                return Json(results);
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
        [HttpGet]
        [Route("api/V1/Clients/List/OrgCode")]
        public async Task<IActionResult> GetClientsByOrgCodeAsync(string orgCode)
        {
            try
            {
                var results = await _clientsService.GetClientsByOrgCodeAsync(orgCode);

                return Json(results);
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
        [HttpPost]
        [Route("api/V1/Clients/Delete")]
        public async Task<IActionResult> DeleteClientsByIdAsync(string orgCode,int id,string clientName)
        {
            try
            {
                var results = await _clientsService.DeleteClientsByIdAsync(orgCode,id, clientName);

                return Json(results);
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
        [HttpPost]
        [Route("api/V1/Clients/Search")]
        public async Task<IActionResult> GetClientsListAsync(ClientsSearchRequest request)
        {
            try
            {
                var results = await _clientsService.GetClientsListAsync(request);
                return Json(results);
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
        [HttpGet]
        [Route("api/V1/Clients/ClientCode")]
        public async Task<IActionResult> GetClientsByClientCodeAsync(string clientCode)
        {
            try
            {
                var results = await _clientsService.GetClientsByClientCodeAsync(clientCode);

                return Json(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
