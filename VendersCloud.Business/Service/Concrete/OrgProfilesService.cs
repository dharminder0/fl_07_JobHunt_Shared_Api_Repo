using VendersCloud.Business.CommonMethods;

namespace VendersCloud.Business.Service.Concrete
{
    public class OrgProfilesService : IOrgProfilesService
    {
        private readonly IOrgProfilesRepository _orgProfilesRepository;
        private readonly IOrgLocationRepository _orgLocationRepository;
        private readonly IListValuesRepository _listValuesRepository;
        private readonly IOrgRelationshipsRepository _orgRelationshipRepository;
        public OrgProfilesService(IOrgProfilesRepository orgProfilesRepository, IOrgLocationRepository orgLocationRepository, IListValuesRepository listValuesRepository, IOrgRelationshipsRepository orgRelationshipRepository)
        {
            _orgProfilesRepository = orgProfilesRepository;
            _orgLocationRepository = orgLocationRepository;
            _listValuesRepository = listValuesRepository;
            _orgRelationshipRepository = orgRelationshipRepository;
        }
        public async Task<bool> AddOrganizationProfileAsync(string orgCode, int profileId)
        {
            try
            {
                if (string.IsNullOrEmpty(orgCode) || profileId < 0)
                {
                    return false;
                }
                var response = await _orgProfilesRepository.AddOrganizationProfileAsync(orgCode, profileId);
                return response;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<PaginationDto<OrganizationDto>> SearchOrganizationsDetails(SearchRequest request)
        {
            try
            {
                var data = await _orgProfilesRepository.SearchOrganizationsDetails(request);
                if (data?.List == null || !data.List.Any())
                    return null;

                // Fetch all OrgCodes from list
                var orgCodes = data.List.Select(x => x.OrgCode).Distinct().ToList();

                if (!orgCodes.Any())
                    return new PaginationDto<OrganizationDto>
                    {
                        Count = 0,
                        Page = request.Page,
                        TotalPages = 0,
                        List = new List<OrganizationDto>()
                    };

                // Fetch locations in a single query
                var orgLocationData = new List<OrgLocation>();

                foreach (var orgCode in orgCodes)
                {
                    var locations = await _orgLocationRepository.GetOrgLocation(orgCode);
                    if (locations != null && locations.Any())
                    {
                        orgLocationData.AddRange(locations);
                    }
                }

                // Fetch all ListValues once for quick lookup
                var listValues = (await _listValuesRepository.GetListValuesAsync())
                    .ToDictionary(x => x.Id, x => x.Value);

                // Map locations by OrgCode
                var orgLocationMap = orgLocationData
                    .GroupBy(x => x.OrgCode)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(loc => new VendersCloud.Business.Entities.ResponseModels.OfficeLocations
                        {
                            City = loc.City,
                            State = loc.State,
                            StateName = listValues.TryGetValue(loc.State, out var stateName) ? stateName : "Unknown"
                        }).ToList()
                    );
                // Fetch OrgRelationships once and build a map of OrgCode -> Status
                var orgRelationshipData = await _orgRelationshipRepository.GetOrgRelationshipsListAsync(request.OrgCode);

                // Build a map of OrgCode -> Status considering both OrgCode and RelatedOrgCode
                var orgStatusMap = new Dictionary<string, int>();

                foreach (var rel in orgRelationshipData)
                {
                    if (!orgStatusMap.ContainsKey(rel.OrgCode))
                        orgStatusMap[rel.OrgCode] = rel.Status;

                    if (!orgStatusMap.ContainsKey(rel.RelatedOrgCode))
                        orgStatusMap[rel.RelatedOrgCode] = rel.Status;
                }

                // Convert Organization to OrganizationDto
                var organizationDtos = data.List.Select(org => new OrganizationDto
                {
                    Id = org.Id,
                    OrgCode = org.OrgCode,
                    OrgName = org.OrgName,
                    Phone = org.Phone,
                    Email = org.Email,
                    CreatedOn = org.CreatedOn,
                    UpdatedOn = org.UpdatedOn,
                    Website = org.Website,
                    EmpCount = org.EmpCount,
                    Logo = org.Logo,
                    Description = org.Description,
                    RegAddress = org.RegAddress,
                    IsDeleted = org.IsDeleted,
                    Location = orgLocationMap.TryGetValue(org.OrgCode, out var locations) ? locations.Select(l => l.City).ToList() : new List<string>(),
                    State = orgLocationMap.TryGetValue(org.OrgCode, out var location) ? location.Select(l => l.StateName).ToList() : new List<string>(),
                    Status = orgStatusMap.TryGetValue(org.OrgCode, out var status) ? status : 0,
                    StatusName= CommonFunctions.GetEnumDescription((InviteStatus) status) 
                }).ToList();

                organizationDtos = organizationDtos
                 .Where(o => o.OrgCode != request.OrgCode)
                 .ToList();
                return new PaginationDto<OrganizationDto>
                {
                    Count = data.Count,
                    Page = data.Page,
                    TotalPages = data.TotalPages,
                    List = organizationDtos
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching organization details", ex);
            }
        }

    }

}
