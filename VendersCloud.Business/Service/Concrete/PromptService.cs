using DapperExtensions;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using VendersCloud.Business.Service.Concrete;
using VendersCloud.Business;

public class PromptService : ExternalServiceBase, IPromptService
{
    private readonly IPromptRepository _promptRepository;
    private readonly IPromptExecutionLogRepository _executionLogRepository;
    private readonly ExternalConfigReader _configReader;
    private readonly IConfiguration _configuration;

    private bool _useAzure;
    private readonly string _azureDeploymentId;
    private readonly string _apiVersion;
    private readonly string _openAIModel;
    private readonly string _openAIBaseUrl;
    private readonly string _openAIApiKey;
    private readonly string _azureBaseUrl;
    private readonly string _azureApiKey;

    public PromptService(IPromptRepository promptRepository, IPromptExecutionLogRepository executionLogRepository, IConfiguration configuration)
     : base(
         new ExternalConfigReader(configuration).GetApiKey(),
         new ExternalConfigReader(configuration).GetBaseUrl()
     )
    {
        _promptRepository = promptRepository;
        _executionLogRepository = executionLogRepository;
        _configReader = new ExternalConfigReader(configuration);
        _configuration = configuration;

        _azureDeploymentId = _configReader.GetDeploymentId();
        _apiVersion = _configReader.GetApiVersion();
        _azureBaseUrl = _configReader.GetBaseUrl();
        _azureApiKey = _configReader.GetApiKey();

    }

    public async Task<UpdatedJobPostingResponse> GenerateUpdatedContent(PromptRequest request)
    {
        string stackTrace = "Started";
        try
        {
            
           _useAzure = true;
            var transactionId = Guid.NewGuid().ToString();
            var requestPayload = JsonConvert.SerializeObject(request);
            string responsePayload = null;

            var prompt = await _promptRepository.GetByAsync(new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate>
                {
                    Predicates.Field<Prompts>(p => p.Code, Operator.Eq, request.PromptCode)
                }
            });

            if (prompt == null)
                return null;

            var structuredInput = JsonConvert.SerializeObject(request.PromptJson);
            var promptDetailUpdated = prompt.PromptDetail.Replace("{RawRequirement}", structuredInput);

            object aiRequest;
            string requestUrl;
            Dictionary<string, string> headers;

            if (_useAzure)
            {
                requestUrl = $"{_azureBaseUrl}openai/deployments/{_azureDeploymentId}/chat/completions?api-version={_apiVersion}";
                headers = new Dictionary<string, string>
                {
                    //{ "api-key", _azureApiKey },
                    { "Authorization", $"Bearer {_azureApiKey}" }
                };

                aiRequest = new
                {
                    messages = new[]
                    {
                        new { role = "system", content = prompt.SystemRole },
                        new { role = "user", content = promptDetailUpdated }
                    },
                    max_tokens = prompt.MaxToken,
                    temperature = (double)prompt.Temperature,
                    top_p = (double)prompt.TopP
                };
            }
            else
            {
                requestUrl = $"{_openAIBaseUrl}/chat/completions";
                headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {_openAIApiKey}" }
                };

                aiRequest = new
                {
                    model = _openAIModel,
                    messages = new[]
                    {
                        new { role = "system", content = prompt.SystemRole },
                        new { role = "user", content = promptDetailUpdated }
                    },
                    max_tokens = prompt.MaxToken,
                    temperature = (double)prompt.Temperature,
                    top_p = (double)prompt.TopP
                };
            }
            stackTrace += "Make call";
            var response = await _httpService.PostAsyncV2<object>(requestUrl, aiRequest, headers);
            responsePayload = JsonConvert.SerializeObject(response);

            dynamic responseObj = JsonConvert.DeserializeObject<dynamic>(responsePayload);
            string rawContent = responseObj.choices[0].message.content.ToString();
            string extractedJson = Regex.Replace(rawContent, @"^```json\s*|\s*```$", "", RegexOptions.Multiline);
            JobPostingResponse job = JsonConvert.DeserializeObject<JobPostingResponse>(extractedJson);
            stackTrace += " call end";
            try
            {
                var log = new PromptExecutionLog
                {
                    Request = request.PromptJson,
                    Response = responsePayload,
                    PromptCode = prompt.Code,
                    CreatedBy = request.LoginUserId,
                    CreatedOn = DateTime.UtcNow
                };
                await _executionLogRepository.InsertDapperAsync(log);
            }
            catch (Exception)
            {
                
            }
            stackTrace += " done";
            if (job == null)
                return null;

            return new UpdatedJobPostingResponse
            {
                Title = job.Title,
                Description = job.Description,
                Experience = job.Experience,
                Positions = job.Positions,
                Duration = job.Contract_Period,
                LocationType = job.Location_Type,
                Location = job.Location,
                Remarks = job.Remark,
                Budget = job.Budget
            };

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    
}
