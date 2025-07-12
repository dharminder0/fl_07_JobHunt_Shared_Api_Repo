using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using VendersCloud.Business;

public class RequireAuthorizationFilter : IAuthorizationFilter
{
    private readonly IConfiguration _configuration;
    private readonly ExternalConfigReader _configReader;
    public string BearerKey { get; set; }

    public RequireAuthorizationFilter(IConfiguration configuration)
    {
        _configuration = configuration;
        _configReader = new ExternalConfigReader(configuration);
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Authorization logic (same as before)
        context.HttpContext.Items.Add("RequestId", Guid.NewGuid().ToString());
        var defaultAuthorizationValue = _configReader.GetAuthorizationBearer();
        var bearerAuthorizeValues = !string.IsNullOrWhiteSpace(BearerKey)
            ? BearerKey.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
            : new List<string> { defaultAuthorizationValue };

        if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var value) &&
            bearerAuthorizeValues.Contains(value.First()))
        {
            return; // Authorization successful
        }

        if (!string.IsNullOrWhiteSpace(BearerKey))
        {
            context.Result = new ForbidResult();
        }
        else
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
