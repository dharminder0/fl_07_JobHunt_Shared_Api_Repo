namespace VendersCloud.Data.Repositories.Concrete
{
    public class PromptExecutionLogRepository : StaticBaseRepository<PromptExecutionLog>, IPromptExecutionLogRepository
    {
        public PromptExecutionLogRepository(IConfiguration configuration) : base(configuration)
        {

        }
    
    }
}
