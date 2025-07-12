namespace VendersCloud.Data.Repositories.Concrete
{
    public class PromptRepository : StaticBaseRepository<Prompts>, IPromptRepository
    {
        public PromptRepository(IConfiguration configuration) : base(configuration)
        {
            
                
        }
    }
}
