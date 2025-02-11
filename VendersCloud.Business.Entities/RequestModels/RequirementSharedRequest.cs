namespace VendersCloud.Business.Entities.RequestModels
{
    public class RequirementSharedRequest
    {
        public int RequirementId {  get; set; }
        public int Visibility {  get; set; }
        public List<string> OrgCode {  get; set; }
    }
}
