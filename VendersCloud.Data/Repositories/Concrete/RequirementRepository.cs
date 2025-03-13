namespace VendersCloud.Data.Repositories.Concrete
{
    public class RequirementRepository : StaticBaseRepository<Requirement>,IRequirementRepository
    {
        private readonly IClientsRepository _clientsRepository;
        public RequirementRepository(IConfiguration configuration, IClientsRepository clientsRepository):base(configuration)
        {
            _clientsRepository= clientsRepository;
        }

        public async Task<string> RequirementUpsertAsync(RequirementRequest request,string uniqueId)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Requirement>().TableName;
            var sql = "SELECT * FROM Requirement WHERE Title = @Title AND OrgCode = @OrgCode";

            // Trim and validate input data
            var cleanedTitle = request.Title.Trim();
            var cleanedOrgCode = request.OrgCode.Trim();

            var response = await dbInstance.SelectAsync<Requirement>(sql, new { Title = cleanedTitle, OrgCode = cleanedOrgCode });
            string result = "";

            if (response.Any())
            {
                // Update query
                var updateQuery = new Query(tableName).AsUpdate(new
                {
                    Title = cleanedTitle,
                    OrgCode = cleanedOrgCode,
                    request.Description,
                    request.Experience,
                    request.Budget,
                    request.Positions,
                    request.LocationType,
                    request.Location,
                    request.Duration,
                    request.ClientCode,
                    request.Remarks,
                    request.Status,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = Convert.ToInt32(request.UserId),
                    IsDeleted = false,
                    UniqueId= uniqueId,
                }).Where("Title", cleanedTitle).Where("OrgCode", cleanedOrgCode);

                await dbInstance.ExecuteAsync(updateQuery);

                // Fetch the Id
                var idResponse = await dbInstance.SelectAsync<Requirement>(sql, new { Title = cleanedTitle, OrgCode = cleanedOrgCode });
                result = idResponse.FirstOrDefault()?.Id.ToString() ?? string.Empty;
            }
            else
            {
                // Insert query
                var insertQuery = new Query(tableName).AsInsert(new
                {
                    Title = cleanedTitle,
                    OrgCode = cleanedOrgCode,
                    request.Description,
                    request.Experience,
                    request.Budget,
                    request.Positions,
                    request.LocationType,
                    request.Location,
                    request.ClientCode,
                    request.Duration,
                    request.Remarks,
                    request.Status,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = Convert.ToInt32(request.UserId),
                    IsDeleted = false,
                    UniqueId = uniqueId,
                });

                await dbInstance.ExecuteAsync(insertQuery);

                // Fetch the Id
                var idResponse = await dbInstance.SelectAsync<Requirement>(sql, new { Title = cleanedTitle, OrgCode = cleanedOrgCode });
                result = idResponse.FirstOrDefault()?.UniqueId.ToString() ?? string.Empty;
            }

            return result;
        }




        public async Task<bool> RequirementUpsertV2Async(RequirementDto request, string uniqueId)
        {
                var dbInstance = GetDbInstance();
                var tableName = new Table<Requirement>();
                var sql = "SELECT * FROM Requirement WHERE Title=@Title AND OrgCode=@OrgCode";

                // Trim and validate input data
                var cleanedTitle = request.Title.Trim();
                var cleanedOrgCode = request.OrgCode.Trim();

                var response = await dbInstance.SelectAsync<Requirement>(sql, new { Title = cleanedTitle, OrgCode = cleanedOrgCode });
                if (response.Any())
                {
                    var updateQuery = new Query(tableName.TableName).AsUpdate(new
                    {
                        Title = cleanedTitle,
                        OrgCode = cleanedOrgCode,
                        request.Description,
                        request.Experience,
                        request.Budget,
                        request.Positions,
                        request.LocationType,
                        request.Location,
                        request.Duration,
                        request.ClientCode,
                        request.Remarks,
                        request.Visibility,
                        request.Hot,
                        request.Status,
                        UpdatedOn = DateTime.UtcNow,
                        UpdatedBy = Convert.ToInt32(request.UserId),
                        IsDeleted = false
                    }).Where("Title", cleanedTitle).Where("OrgCode", cleanedOrgCode);
                    await dbInstance.ExecuteScalarAsync<string>(updateQuery);
                }
                else
                {
                    var insertQuery = new Query(tableName.TableName).AsInsert(new
                    {
                        Title = cleanedTitle,
                        OrgCode = cleanedOrgCode,
                        request.Description,
                        request.Experience,
                        request.Budget,
                        request.Positions,
                        request.LocationType,
                        request.Location,
                        request.ClientCode,
                        request.Duration,
                        request.Remarks,
                        request.Visibility,
                        request.Hot,
                        request.Status,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = Convert.ToInt32(request.UserId),
                        IsDeleted = false
                    });
                    await dbInstance.ExecuteScalarAsync<string>(insertQuery);
                }
                return true;
        }

        public async Task<bool> DeleteRequirementAsync(int requirementId, string orgCode)
        {
           
                var dbInstance = GetDbInstance();
                var tableName = new Table<Requirement>();
                var sql = "SELECT * FROM Requirement WHERE Id=@Id AND OrgCode=@OrgCode";

                // Trim and validate input data
                var Id = requirementId;
                var cleanedOrgCode = orgCode.Trim();

                var response = await dbInstance.SelectAsync<Requirement>(sql, new { Id = Id, OrgCode = cleanedOrgCode });
                if (response.Any())
                {
                    var updateQuery = new Query(tableName.TableName).AsUpdate(new
                    { 
                        IsDeleted = true
                    }).Where("Id", Id).Where("OrgCode", cleanedOrgCode);
                    await dbInstance.ExecuteScalarAsync<string>(updateQuery);
                    return true;
                }
                return false;
            
        }

        public async Task<List<Requirement>> GetRequirementListAsync()
        {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM Requirement Where IsDeleted<>1 Order By 1 Desc";

                var list = dbInstance.Select<Requirement>(sql).ToList();
                return list;
            
         
        }

        public async Task<List<Requirement>> GetRequirementListByIdAsync(string requirementId)
        {
           
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM Requirement Where IsDeleted<>1 and UniqueId=@requirementId";

                var list = dbInstance.Select<Requirement>(sql, new { requirementId}).ToList();
                return list;
           
        }

        public async Task<IEnumerable<Requirement>> GetRequirementByIdAsync(List<int> requirementId)
        {

            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Requirement Where IsDeleted<>1 and Id In @requirementId";

            var data = dbInstance.Select<Requirement>(sql, new { requirementId });
            return data;

        }
        public async Task<bool> UpdateStatusByIdAsync(int requirementId, int status)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Requirement>();
            var insertQuery = new Query(tableName.TableName)
                .AsUpdate(new
                {
                    Status=status,
                    IsDeleted = false
                })
                .Where("Id", requirementId);
            await dbInstance.ExecuteAsync(insertQuery);
            return true;
        }

        public async Task<List<Requirement>> GetRequirementByOrgCodeAsync(string orgCode)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Requirement Where IsDeleted<>1 and OrgCode=@orgCode";

            var list = dbInstance.Select<Requirement>(sql, new { orgCode }).ToList();
            return list;

        }

        public async Task<List<Requirement>> GetRequirementsListByVisibilityAsync(SearchRequirementRequest request)
        {
            using var connection = GetConnection();
            var predicates = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                predicates.Add("(r.Title LIKE @searchText OR r.Description LIKE @searchText OR r.ClientCode LIKE @searchText)");
                parameters.Add("searchText", $"%{request.SearchText}%");
            }

            if (request.LocationType != null && request.LocationType.Any())
            {
                predicates.Add("r.LocationType IN @locationTypes");
                parameters.Add("locationTypes", request.LocationType);
            }

            if (!string.IsNullOrEmpty(request.UserId) && request.RoleType.Any() && request.RoleType != null)
            {
                var rolePlaceholders = string.Join(", ", request.RoleType.Select((role, index) => $"@Role{index}"));
                predicates.Add($"EXISTS (SELECT 1 FROM UserProfiles op WHERE  op.ProfileId IN ({rolePlaceholders}))");

                for (int i = 0; i < request.RoleType.Count; i++)
                {
                    parameters.Add($"Role{i}", request.RoleType[i]);
                }
                parameters.Add("IsDeleted", false);
            }
            if (request.Status != null && request.Status.Any())
            {
                predicates.Add("r.Status IN @statuses");
                parameters.Add("statuses", request.Status);
            }

            if (request.ClientCode != null && request.ClientCode.Any())
            {
                predicates.Add("r.ClientCode IN @clientCodes");
                parameters.Add("clientCodes", request.ClientCode);
            }

            predicates.Add("r.IsDeleted = 0");
            predicates.Add("r.OrgCode <> @orgCode");
            parameters.Add("orgCode", request.OrgCode);
            predicates.Add("r.Visibility='3'");
            

            string whereClause = predicates.Any() ? "WHERE " + string.Join(" AND ", predicates) : "";

            string query = $@"
SELECT * FROM Requirement r
{whereClause}
ORDER BY r.CreatedOn DESC;";

            using var multi = await connection.QueryMultipleAsync(query, parameters);
            var requirements = (await multi.ReadAsync<Requirement>()).ToList();
            return requirements;
        }

        public async Task<List<Requirement>> GetRequirementsListAsync(SearchRequirementRequest request)
        {
            using var connection = GetConnection();
            var predicates = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                predicates.Add("(r.Title LIKE @searchText OR r.Description LIKE @searchText OR r.ClientCode LIKE @searchText)");
                parameters.Add("searchText", $"%{request.SearchText}%");
            }

            if (request.LocationType != null && request.LocationType.Any())
            {
                predicates.Add("r.LocationType IN @locationTypes");
                parameters.Add("locationTypes", request.LocationType);
            }

            if (!string.IsNullOrEmpty(request.UserId) && request.RoleType.Any() && request.RoleType != null)
            {
                var rolePlaceholders = string.Join(", ", request.RoleType.Select((role, index) => $"@Role{index}"));
                predicates.Add($"EXISTS (SELECT 1 FROM UserProfiles op WHERE op.UserId = @UserId AND op.ProfileId IN ({rolePlaceholders}))");

                for (int i = 0; i < request.RoleType.Count; i++)
                {
                    parameters.Add($"Role{i}", request.RoleType[i]);
                }
                parameters.Add("IsDeleted", false);
                parameters.Add("UserId", Convert.ToInt32(request.UserId));
            }
            if (request.Status != null && request.Status.Any())
            {
                predicates.Add("r.Status IN @statuses");
                parameters.Add("statuses", request.Status);
            }

            if (request.ClientCode != null && request.ClientCode.Any())
            {
                predicates.Add("r.ClientCode IN @clientCodes");
                parameters.Add("clientCodes", request.ClientCode);
            }

            predicates.Add("r.IsDeleted = 0");
            predicates.Add("r.OrgCode = @orgCode");
            parameters.Add("orgCode", request.OrgCode);

            string whereClause = predicates.Any() ? "WHERE " + string.Join(" AND ", predicates) : "";

            string query = $@"
SELECT * FROM Requirement r
{whereClause}
ORDER BY r.CreatedOn DESC;";

            using var multi = await connection.QueryMultipleAsync(query, parameters);
            var requirements = (await multi.ReadAsync<Requirement>()).ToList();
            return requirements;
        }

    }
}
