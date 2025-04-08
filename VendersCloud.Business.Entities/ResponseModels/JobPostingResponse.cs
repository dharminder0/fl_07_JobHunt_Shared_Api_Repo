using Newtonsoft.Json;

namespace VendersCloud.Business.Entities.ResponseModels
{
    public class JobPostingResponse
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Experience { get; set; }
        public string Budget { get; set; }
        public string Positions { get; set; }
        public string Contract_Period { get; set; }
        public string Location_Type { get; set; }
        public string Location { get; set; }
        public string Remark { get; set; }
        public List<string> Skills { get; set; }
    }

    public class UpdatedJobPostingResponse
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Experience { get; set; }
        public string Budget { get; set; }
        public string Positions { get; set; }
        public string Duration { get; set; }
        public string LocationType { get; set; }
        public string Location { get; set; }
        public string Remarks { get; set; }
        public List<string> Skills { get; set; }


    }
}
