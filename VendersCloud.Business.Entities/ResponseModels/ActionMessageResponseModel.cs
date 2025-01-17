namespace VendersCloud.Business.Entities.ResponseModels
{
    public class ActionMessageResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Content { get; set; }
    }
}
