namespace VendersCloud.Business.Entities.ResponseModels
{
    public class CompanyDashboardCountResponse
    {
        public int OpenPositions { get; set; }
        public int HotRequirements { get; set; }
        public int InterviewScheduled { get; set; }
        public int CandidatesToReview { get; set; }
        public int TotalApplicants { get; set; }
        public int NoApplications { get; set; }

    }
}
