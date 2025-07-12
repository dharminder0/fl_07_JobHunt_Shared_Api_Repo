using Microsoft.AspNetCore.Mvc;

namespace VendersCloud.Business.Service.Concrete
{
    public class MatchRecordService : IMatchRecordService
    {
        private readonly IMatchRecordRepository _matchRecordRepository;
        private readonly IBenchRepository _benchRepository;
        private readonly IRequirementRepository _requirementRepository;
        public MatchRecordService(IMatchRecordRepository matchRecordRepository, IBenchRepository benchRepository, IRequirementRepository requirementRepository)
        {
            _matchRecordRepository = matchRecordRepository;
            _benchRepository = benchRepository;
            _requirementRepository = requirementRepository;
        }


        public async Task<List<dynamic>> GetMatchRecordAsync(MatchRecordRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (request.ResourceId.Count == 0 && request.RequirementId.Count != 0)
            {
                var data=  await _matchRecordRepository.GetMatchRecordByRequirementIdAsync(request.RequirementId,request.MatchScore);
                //List<dynamic> result = new List<dynamic>();
                //foreach (var item in data)
                //{
                //    var resourceIds = ((string)item.ResourceIds).Split(',');
                //    List<int> resourceIdsList = resourceIds.Select(x => int.Parse(x)).ToList();

                //    var count = await _benchRepository.GetBenchResponseListByIdAsync(resourceIdsList);
                //    result.Add(new { item.RequirementId,item.TotalCandidates,MatchingScore=item.MatchScore, count });
                //}
                //return result;
                List<dynamic> result = new List<dynamic>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        bool Success = true;
                        result.Add(new { item.RequirementId, item.TotalCandidates, MatchingScore = item.MatchScore,item.ResourceIds, Success });
                    }
                }
                return result;
            }
            else if (request.RequirementId.Count <= 0 && request.ResourceId.Count != 0)
            {
                var data= await _matchRecordRepository.GetMatchRecordByResourceIdAsync(request.ResourceId, request.MatchScore);
                //List<dynamic> result = new List<dynamic>();
                //foreach (var item in data)
                //{
                //    var requirementIds = ((string)item.RequirementIds).Split(',');
                //    List<int> requirementIdsList = requirementIds.Select(x => int.Parse(x)).ToList();
                //    var count = await _requirementRepository.GetRequirementByIdAsync(requirementIdsList);
                //    result.Add(new { item.ResourceId, item.MatchingRequirements, MatchingScore = item.MatchScore, count });
                //}
                //return result;
                List<dynamic> result = new List<dynamic>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        bool Success = true;
                        result.Add(new { item.RequirementIds, item.MatchingRequirements, MatchingScore = item.MatchScore, item.ResourceId, Success });
                    }
                }
                return result;

            }
            else if (request.RequirementId.Count > 0 && request.ResourceId.Count > 0)
            {
                return await _matchRecordRepository.GetMatchRecordByResourceAndRequirementIdAsync(request.ResourceId, request.RequirementId,request.MatchScore);
            }
            else
            {
                throw new ArgumentException("Invalid input");
            }
        }
    }
}
