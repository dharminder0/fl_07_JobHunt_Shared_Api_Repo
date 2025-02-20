using DapperExtensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using VendersCloud.Business;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Business.Service.Concrete;
using VendersCloud.Data.Repositories.Abstract;

public class PromptService : ExternalServiceBase, IPromptService
{
    private readonly IPromptRepository _promptRepository;
    private readonly IPromptExecutionLogRepository _executionLogRepository;
    private readonly ExternalConfigReader _configReader;

    public PromptService(IPromptRepository promptRepository, IPromptExecutionLogRepository executionLogRepository, IConfiguration configuration)
        : base(configuration["OpenAI:BaseUrl"], configuration["OpenAI:ApiKey"])
    {
        _promptRepository = promptRepository;
        _executionLogRepository = executionLogRepository;

        // Initialize ExternalConfigReader to read the external appsettings.json
        _configReader = new ExternalConfigReader(configuration);
    }

    public async Task<UpdatedJobPostingResponse> GenerateUpdatedContent(PromptRequest request)
    {
        try
        {
            // First, try to get values from appsettings.json (and fallback to external file if needed)
            string apiKey = _configReader.GetApiKey();
            string baseUrl = _configReader.GetBaseUrl();

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
            {
                return null;
            }

            var structuredInput = JsonConvert.SerializeObject(request.PromptJson);
            var promptDetailUpdated = prompt.PromptDetail.Replace("{RawRequirement}", structuredInput);

            var aiRequest = new
            {
                model = prompt.Model,
                messages = new[]
                {
                    new { role = "system", content = prompt.SystemRole },
                    new { role = "user", content = promptDetailUpdated }
                },
                max_tokens = prompt.MaxToken,

                temperature = (double)prompt.Temperature,
                top_p = (double)prompt.TopP,

            };

            var headers = new Dictionary<string, string>
            {
                { "Authorization", apiKey },
               
            };

            var response = await _httpService.PostAsyncV2<object>(baseUrl, aiRequest, headers);
            responsePayload = JsonConvert.SerializeObject(response);

            dynamic responseObj = JsonConvert.DeserializeObject<dynamic>(responsePayload);
            string rawContent = responseObj.choices[0].message.content.ToString();
            string extractedJson = Regex.Replace(rawContent, @"^```json\s*|\s*```$", "", RegexOptions.Multiline);
            JobPostingResponse job = JsonConvert.DeserializeObject<JobPostingResponse>(extractedJson);

            // Insert log into execution log repository
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
                // Handle any exception that occurs during logging
            }
            UpdatedJobPostingResponse res = new UpdatedJobPostingResponse();
            if (job!=null)
            {
                res.Title = job.Title;
                res.Description = job.Description;
                res.Experience = job.Experience;
                res.Positions = job.Positions;
                res.Duration = job.Contract_Period;
                res.LocationType = job.Location_Type;
                res.Location = job.Location;
                res.Remarks = job.Remark;
                res.Budget = job.Budget;
            }
            return res;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }
}
