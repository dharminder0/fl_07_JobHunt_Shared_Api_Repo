using VendersCloud.Business.Entities.DTOModels;

namespace VendersCloud.Business.Entities.Dtos
{
    public  class CompanyUserListDto
    {
        public int Id { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Email {  get; set; }
        public DateTime CreatedOn {  get; set; }
        public DateTime UpdatedOn { get; set; }
        public string CompanyStrength {  get; set; }
        public string CompanyWebsite {  get; set; }
        public string CompanyIcon { get; set; }
        public string Description {  get; set; }
        public List<UserDto> Users {  get; set; }
    }
}
