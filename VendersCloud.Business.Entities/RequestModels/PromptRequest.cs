namespace VendersCloud.Business.Entities.RequestModels
{
    public class PromptRequest
    {
        public string  PromptCode { get; set; }
        public int LoginUserId { get; set; }
        public string PromptJson { get; set; }
    }
}
