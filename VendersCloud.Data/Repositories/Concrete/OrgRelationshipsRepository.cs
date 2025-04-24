namespace VendersCloud.Data.Repositories.Concrete
{
    public class OrgRelationshipsRepository : StaticBaseRepository<OrgRelationships>, IOrgRelationshipsRepository
    {
        private readonly IOrgLocationRepository _organizationLocationRepository;
        private readonly IOrganizationRepository _organizationRepository;
        public OrgRelationshipsRepository(IConfiguration configuration, IOrgLocationRepository organizationLocationRepository, IOrganizationRepository organizationRepository) : base(configuration)
        {
            _organizationLocationRepository = organizationLocationRepository;
            _organizationRepository = organizationRepository;
        }

        public async Task<bool> AddOrgRelationshipDataAsync(string orgCode,string relatedOrgCode,string relationshipType,int status,int createdBy)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<OrgRelationships>();

            var query = new Query(tableName.TableName)
                .Where("IsDeleted", false)
                .Where("OrgCode", orgCode)
                .Where("RelatedOrgCode",relatedOrgCode)
                .Where("relationshipType",relationshipType)
                .Select("Id");

            var existingOrgCode = await dbInstance.ExecuteScalarAsync<string>(query);
            if (!string.IsNullOrEmpty(existingOrgCode))
            {
                var updateQuery = new Query(tableName.TableName).AsUpdate(new
                {
                    OrgCode = orgCode,
                    RelatedOrgCode = relatedOrgCode,
                    RelationshipType = relationshipType,
                    Status = status,
                    UpdatedBy = createdBy,
                    UpdatedOn = DateTime.UtcNow,
                    IsDeleted = false
                }).Where("Id", existingOrgCode);
                await dbInstance.ExecuteScalarAsync<string>(updateQuery);
                return true;
            }
            var insertQuery = new Query(tableName.TableName).AsInsert(new
            {
                OrgCode = orgCode,
                RelatedOrgCode = relatedOrgCode,
                RelationshipType = relationshipType,
                Status = status,
                CreatedBy=createdBy,
                UpdatedBy= createdBy,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                IsDeleted = false
            });

            var insertedOrgCode = await dbInstance.ExecuteScalarAsync<string>(insertQuery);
            return true;
        }

        public async Task<bool> ManageRelationshipStatusAsync(int orgRelationshipId, int status)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<OrgRelationships>();
            var updateQuery = new Query(tableName.TableName).AsUpdate(new
            {
                Status = status,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                IsDeleted = false
            }).Where("Id", orgRelationshipId);

            var insertedOrgCode = await dbInstance.ExecuteScalarAsync<string>(updateQuery);
            return true;
        }

        public async Task<PaginationDto<OrgRelationshipSearchResponse>> GetListRelationshipAsync(OrgRelationshipSearchRequest request)
        {
            using var connection = GetConnection();
            var predicates = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(request.searchText))
            {
                predicates.Add("(r.OrgCode LIKE @SearchText)");
                parameters.Add("SearchText", $"%{request.searchText}%");
            }

            if (request.Status != null)
            {
                predicates.Add("r.Status = @statuses");
                parameters.Add("statuses", request.Status);
            }

            predicates.Add("r.IsDeleted = 0");

            if (!string.IsNullOrWhiteSpace(request.OrgCode))
            {
                predicates.Add("(r.OrgCode = @orgCode Or r.RelatedOrgCode=@orgCode)");
                parameters.Add("orgCode", request.OrgCode);
            }

            if (!string.IsNullOrWhiteSpace(request.RelatedOrgCode))
            {
                predicates.Add("(r.RelatedOrgCode = @relatedOrgCode Or r.OrgCode= @relatedOrgCode)");
                parameters.Add("relatedOrgCode", request.RelatedOrgCode);
            }
            if (request.RelationshipType?.Any() == true)
            {
                predicates.Add("r.RelationshipType = @relationshipType");
                parameters.Add("relationshipType", request.RelationshipType);
            }
            string whereClause = predicates.Any() ? "WHERE " + string.Join(" AND ", predicates) : "";
            string query = $@"
    SELECT * FROM OrgRelationships r 
    {whereClause} 
    ORDER BY r.CreatedOn DESC 
    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
    
    SELECT COUNT(*) FROM OrgRelationships r {whereClause};
    ";

            parameters.Add("offset", (request.Page - 1) * request.PageSize);
            parameters.Add("pageSize", request.PageSize);

            using var multi = await connection.QueryMultipleAsync(query, parameters);
            var relationships = (await multi.ReadAsync<OrgRelationships>()).ToList();
            int totalRecords = await multi.ReadFirstOrDefaultAsync<int>();

            var responseList = new List<OrgRelationshipSearchResponse>();

            foreach (var relationship in relationships)
            {
                List<Organization> selectedOrgDataList = new();
                List<OrgLocation> selectedOrgLocationData = new();

                if (!string.IsNullOrWhiteSpace(request.OrgCode))
                {
                    var orgDataResult = await _organizationRepository.GetOrganizationData(relationship.RelatedOrgCode);
                    if (orgDataResult is IEnumerable<Organization> orgList)
                        selectedOrgDataList = orgList.ToList();
                    else if (orgDataResult != null)
                        selectedOrgDataList.Add(orgDataResult);

                    var locationResult = await _organizationLocationRepository.GetOrgLocation(relationship.RelatedOrgCode);
                    selectedOrgLocationData = locationResult?.ToList() ?? new List<OrgLocation>();
                }
                else if (!string.IsNullOrWhiteSpace(request.RelatedOrgCode))
                {
                    var orgDataResult = await _organizationRepository.GetOrganizationData(relationship.OrgCode);
                    if (orgDataResult is IEnumerable<Organization> orgList)
                        selectedOrgDataList = orgList.ToList();
                    else if (orgDataResult != null)
                        selectedOrgDataList.Add(orgDataResult);

                    var locationResult = await _organizationLocationRepository.GetOrgLocation(relationship.OrgCode);
                    selectedOrgLocationData = locationResult?.ToList() ?? new List<OrgLocation>();
                }

                foreach (var orgData in selectedOrgDataList)
                {
                    responseList.Add(new OrgRelationshipSearchResponse
                    {
                        Id = relationship.Id,
                        OrgCode = relationship.OrgCode,
                        RelatedOrgCode = relationship.RelatedOrgCode,
                        RelationshipType = relationship.RelationshipType,
                        StatusName = System.Enum.GetName(typeof(InviteStatus), relationship.Status),
                        Status = relationship.Status,
                        Description = orgData.Description,
                        OrgName = orgData.OrgName,
                        EmpCount = orgData.EmpCount,
                        Logo = orgData.Logo,
                        Location = selectedOrgLocationData.Select(loc => loc.City).Distinct().ToList(),
                        CreatedBy = relationship.CreatedBy,
                        UpdatedBy = relationship.UpdatedBy,
                        CreatedOn = relationship.CreatedOn,
                        UpdatedOn = relationship.UpdatedOn,
                        IsDeleted = relationship.IsDeleted
                    });
                }
            }

            return new PaginationDto<OrgRelationshipSearchResponse>
            {
                Count = totalRecords,
                Page = request.Page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                List = responseList
            };
        }

        public async Task<IEnumerable<OrgRelationships>> GetBenchResponseListByIdAsync(string orgCode)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM OrgRelationships Where IsDeleted<>1 and (OrgCode = @orgCode OR RelatedOrgCode=@orgCode) And Status=2";

            var list = dbInstance.Select<OrgRelationships>(sql, new { orgCode }).ToList();
            return list;
        }

        public async Task<List<OrgRelationships>> GetOrgRelationshipsListAsync(string orgCode ,int role)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM OrgRelationships Where IsDeleted<>1 and (OrgCode = @orgCode OR RelatedOrgCode=@orgCode) and RelationshipType<>@role";

            var list = dbInstance.Select<OrgRelationships>(sql, new { orgCode,role }).ToList();
            return list;
        }
        
        public async Task<IEnumerable<OrgRelationships>> GetStatusAsync(string orgCode,string relatedOrgCode)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM OrgRelationships Where IsDeleted<>1 and (OrgCode = @orgCode OR RelatedOrgCode=@orgCode) And(OrgCode = @relatedOrgCode OR RelatedOrgCode=@relatedOrgCode)";

            var list = dbInstance.Select<OrgRelationships>(sql, new { orgCode,relatedOrgCode }).ToList();
            return list;
        }
    }
}
 