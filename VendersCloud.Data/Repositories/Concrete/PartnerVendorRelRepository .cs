using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class PartnerVendorRelRepository : StaticBaseRepository<PartnerVendorRel>, IPartnerVendorRelRepository
    {
        private readonly IOrgLocationRepository _organizationLocationRepository;
        private readonly IOrganizationRepository _organizationRepository;
        public PartnerVendorRelRepository(IConfiguration configuration, IOrgLocationRepository organizationLocationRepository, IOrganizationRepository organizationRepository) : base(configuration) {

            _organizationLocationRepository = organizationLocationRepository;
            _organizationRepository = organizationRepository;
        }

        public async Task<PartnerVendorRel> GetByIdAsync(int id)
        {
            try
            {
                return await GetByAsync(new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate>
                    {
                        Predicates.Field<PartnerVendorRel>(f => f.Id, Operator.Eq, id),
                        Predicates.Field<PartnerVendorRel>(f => f.IsDeleted, Operator.Eq, false),
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }
        public async Task<PartnerVendorRel> ManagePartnerStatusAsync(string partnerCode, string vendorCode)
        {
            try
            {
                var predicateGroup = new PredicateGroup
                {
                    Operator = GroupOperator.Or,
                    Predicates = new List<IPredicate>
            {
                new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate>
                    {
                        Predicates.Field<PartnerVendorRel>(f => f.PartnerCode, Operator.Eq, partnerCode),
                        Predicates.Field<PartnerVendorRel>(f => f.VendorCode, Operator.Eq, vendorCode),
                        Predicates.Field<PartnerVendorRel>(f => f.IsDeleted, Operator.Eq, false),
                    }
                },
                new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate>
                    {
                        Predicates.Field<PartnerVendorRel>(f => f.PartnerCode, Operator.Eq, vendorCode),
                        Predicates.Field<PartnerVendorRel>(f => f.VendorCode, Operator.Eq, partnerCode),
                        Predicates.Field<PartnerVendorRel>(f => f.IsDeleted, Operator.Eq, false),
                    }
                }
            }
                };

                return await GetByAsync(predicateGroup);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<List<string>> GetAllVendorCodeAsync(string partnerCode)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var sql = "SELECT vendorCode FROM PartnerVendorRel WHERE IsDeleted = 0 and partnerCode= @partnerCode and statusId=2 ";
                var result = await dbInstance.SelectAsync<string>(sql, new { partnerCode });
                return result.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }


        public async Task<bool> UpdatePartnerVendorRelByIdAsync(int id, PartnerVendorRel updatedEntity)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var tableName = new Table<PartnerVendorRel>();

                var updateQuery = new Query(tableName.TableName).AsUpdate(new
                {
                   
                    PartnerCode = updatedEntity.PartnerCode,
                    VendorCode = updatedEntity.VendorCode,
                    UpdatedOn = DateTime.UtcNow,
                    IsDeleted = false,
                    CreatedBy = updatedEntity.CreatedBy,
                    UpdatedBy = updatedEntity.UpdatedBy,
                    StatusId = updatedEntity.StatusId  ,                    
                    CreatedOn = updatedEntity.CreatedOn,
                 
                }).Where("Id", id);

                var res = await dbInstance.ExecuteAsync(updateQuery);
                return res != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }
        public async Task<int> UpsertPartnerVendorRelAsync(PartnerVendorRel entity)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var tableName = new Table<PartnerVendorRel>();

                // Convert the checkQuery to a SQLKata Query
                var query = new Query(tableName.TableName)
                    .Select("Id")
                    .Where("PartnerCode", entity.PartnerCode)
                    .Where("VendorCode", entity.VendorCode)
                    .Where("IsDeleted", false)
                    .Limit(1);

                // Use ExecuteScalarAsync to check if the record exists
                var existingId = await dbInstance.ExecuteScalarAsync<int?>(query);

                if (existingId.HasValue)
                {
                    // Record exists, update it
                    var updateQuery = new Query(tableName.TableName)
                        .AsUpdate(new
                        {
                            UpdatedOn = DateTime.UtcNow,
                            UpdatedBy = entity.UpdatedBy,
                            StatusId = entity.StatusId
                        })
                        .Where("Id", existingId.Value);

                    await dbInstance.ExecuteAsync(updateQuery);
                    return existingId.Value;
                }
                else
                {
                    // Insert new record
                    var insertQuery = new Query(tableName.TableName).AsInsert(new
                    {
                        entity.PartnerCode,
                        entity.VendorCode,
                        CreatedOn = entity.CreatedOn,
                        CreatedBy = entity.CreatedBy,
                        UpdatedBy = entity.UpdatedBy,
                        UpdatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        entity.StatusId
                    });

                    int insertedId = await dbInstance.ExecuteScalarAsync<int>(insertQuery);
                    return insertedId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return 0;
            }
        }



        public async Task<List<PartnerVendorRel>> GetByPartnerIdAsync(string partnerCode, string vendorCode)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM PartnerVendorRel WHERE PartnerCode = @PartnerCode  and vendorCode = @vendorCode AND IsDeleted = 0";

                var result = await dbInstance.SelectAsync<PartnerVendorRel>(sql, new { partnerCode,vendorCode });
                return result.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> ManagePartnerStatusAsync(ManageRelationshipStatusRequest manageRelationship)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<PartnerVendorRel>();
            var updateQuery = new Query(tableName.TableName).AsUpdate(new
            {
                StatusId = manageRelationship.StatusId,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                IsDeleted = false,
                UpdatedBy = manageRelationship.UpdatedBy
            }).Where("Id", manageRelationship.PartnerVendorRelId);

            var insertedOrgCode = await dbInstance.ExecuteScalarAsync<string>(updateQuery);
            return true;
        }
        public async Task<List<PartnerVendorRel>> GetOrgRelationshipsListAsync(string orgCode)
        {
           
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM PartnerVendorRel Where IsDeleted<>1 and (PartnerCode = @orgCode OR vendorCode=@orgCode) ";

            var list = dbInstance.Select<PartnerVendorRel>(sql, new { orgCode }).ToList();
            return list;
        }
        public async Task<PaginationDto<OrgRelationshipSearchResponse>> GetListRelationshipAsync(OrgRelationshipSearchRequest request)
        {
            using var connection = GetConnection();
            var predicates = new List<string>();
            var parameters = new DynamicParameters();



            if (!string.IsNullOrWhiteSpace(request.searchText))
            {
                predicates.Add("(r.PartnerCode LIKE @SearchText OR r.VendorCode LIKE @SearchText OR po.OrgName LIKE @SearchText OR vo.OrgName LIKE @SearchText)");
                parameters.Add("SearchText", $"%{request.searchText}%");
            }



            if (request.Status != null)
            {
                predicates.Add("r.StatusId = @statuses");
                parameters.Add("statuses", request.Status);
            }



            predicates.Add("r.IsDeleted = 0");



            if (!string.IsNullOrWhiteSpace(request.OrgCode))
            {
                predicates.Add("(r.PartnerCode = @orgCode OR r.VendorCode = @orgCode)");
                parameters.Add("orgCode", request.OrgCode);
            }



            if (!string.IsNullOrWhiteSpace(request.RelatedOrgCode))
            {
                predicates.Add("(r.VendorCode = @relatedOrgCode OR r.PartnerCode = @relatedOrgCode)");
                parameters.Add("relatedOrgCode", request.RelatedOrgCode);
            }



            string whereClause = predicates.Any() ? "WHERE " + string.Join(" AND ", predicates) : "";
            string query = $@"
SELECT r.* 
FROM PartnerVendorRel r 
LEFT JOIN Organization po ON r.PartnerCode = po.OrgCode
LEFT JOIN Organization vo ON r.VendorCode = vo.OrgCode
{whereClause} 
ORDER BY r.CreatedOn DESC 
OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;



SELECT COUNT(*) 
FROM PartnerVendorRel r 
LEFT JOIN Organization po ON r.PartnerCode = po.OrgCode
LEFT JOIN Organization vo ON r.VendorCode = vo.OrgCode
{whereClause};";



            parameters.Add("offset", (request.Page - 1) * request.PageSize);
            parameters.Add("pageSize", request.PageSize);



            using var multi = await connection.QueryMultipleAsync(query, parameters);
            var relationships = (await multi.ReadAsync<PartnerVendorRel>()).ToList();
            if (!string.IsNullOrWhiteSpace(request.RelatedOrgCode))
            {
                relationships = relationships
                .Where(v => v.PartnerCode != request.RelatedOrgCode)
                .ToList();



            }



            if (!string.IsNullOrWhiteSpace(request.OrgCode))
            {
                relationships = relationships
                .Where(v => v.VendorCode != request.OrgCode)
                .ToList();



            }



            int totalRecords = await multi.ReadFirstOrDefaultAsync<int>();



            var responseList = new List<OrgRelationshipSearchResponse>();



            foreach (var relationship in relationships)
            {
                List<Organization> selectedOrgDataList = new();
                List<OrgLocation> selectedOrgLocationData = new();



                if (!string.IsNullOrWhiteSpace(request.OrgCode))
                {
                    var orgDataResult = await _organizationRepository.GetOrganizationData(relationship.VendorCode);
                    if (orgDataResult is IEnumerable<Organization> orgList)
                        selectedOrgDataList = orgList.ToList();
                    else if (orgDataResult != null)
                        selectedOrgDataList.Add(orgDataResult);



                    var locationResult = await _organizationLocationRepository.GetOrgLocation(relationship.VendorCode);
                    selectedOrgLocationData = locationResult?.ToList() ?? new List<OrgLocation>();
                }
                else if (!string.IsNullOrWhiteSpace(request.RelatedOrgCode))
                {
                    var orgDataResult = await _organizationRepository.GetOrganizationData(relationship.PartnerCode);
                    if (orgDataResult is IEnumerable<Organization> orgList)
                        selectedOrgDataList = orgList.ToList();
                    else if (orgDataResult != null)
                        selectedOrgDataList.Add(orgDataResult);



                    var locationResult = await _organizationLocationRepository.GetOrgLocation(relationship.PartnerCode);
                    selectedOrgLocationData = locationResult?.ToList() ?? new List<OrgLocation>();
                }



                foreach (var orgData in selectedOrgDataList)
                {
                    responseList.Add(new OrgRelationshipSearchResponse
                    {
                        Id = relationship.Id,
                        OrgCode = relationship.PartnerCode,
                        RelatedOrgCode = relationship.VendorCode,
                        RelationshipType = relationship.StatusId.ToString(),  // Assuming StatusId is mapped to relationshipType
                        StatusName = System.Enum.GetName(typeof(InviteStatus), relationship.StatusId),
                        Status = relationship.StatusId,
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
        public async Task<IEnumerable<PartnerVendorRel>> GetBenchResponseListByIdAsync(string orgCode)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM PartnerVendorRel Where IsDeleted<>1 and (PartnerCode = @orgCode OR vendorCode=@orgCode) And StatusId=2";

            var list = dbInstance.Select<PartnerVendorRel>(sql, new { orgCode }).ToList();
            return list;
        }
    }
}
