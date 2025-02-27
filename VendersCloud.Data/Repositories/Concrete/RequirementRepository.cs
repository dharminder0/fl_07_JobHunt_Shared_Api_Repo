using Dapper;
using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Data.Data;
using VendersCloud.Data.Repositories.Abstract;
using static VendersCloud.Data.Enum.Enum;

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
                    UpdatedBy = request.UserId,
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
                    CreatedBy = request.UserId,
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
                        UpdatedBy = request.UserId,
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
                        CreatedBy = request.UserId,
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

        public async Task<PaginationDto<RequirementResponse>> GetRequirementsListAsync(SearchRequirementRequest request)
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
                ORDER BY r.CreatedOn DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                
                SELECT COUNT(*) FROM Requirement r {whereClause};";

            parameters.Add("offset", (request.Page - 1) * request.PageSize);
            parameters.Add("pageSize", request.PageSize);

            using var multi = await connection.QueryMultipleAsync(query, parameters);
            var requirements = (await multi.ReadAsync<Requirement>()).ToList();
            int totalRecords = await multi.ReadFirstOrDefaultAsync<int>();

            var requirementsResponseList = new List<RequirementResponse>();

            foreach (var r in requirements)
            {
                var requirementResponse = new RequirementResponse
                {
                    Id = r.Id,
                    Title = r.Title,
                    OrgCode = r.OrgCode,
                    Description = r.Description,
                    Experience = r.Experience,
                    Budget = r.Budget,
                    Positions = r.Positions,
                    Duration = r.Duration,
                    LocationType = r.LocationType,
                    LocationTypeName = System.Enum.GetName(typeof(LocationType),r.LocationType),
                    Location = r.Location,
                    ClientCode = r.ClientCode,
                    Remarks = r.Remarks,
                    Visibility = r.Visibility,
                    VisibilityName = System.Enum.GetName(typeof(Visibility), r.Visibility),
                    Hot = r.Hot,
                    Status = r.Status,
                    StatusName = System.Enum.GetName(typeof(RequirementsStatus), r.Status),
                    CreatedOn = r.CreatedOn,
                    UpdatedOn = r.UpdatedOn,
                    CreatedBy = r.CreatedBy,
                    UpdatedBy = r.UpdatedBy,
                    IsDeleted = r.IsDeleted,
                    UniqueId = r.UniqueId
                };

                var orgData = await _clientsRepository.GetClientsByClientCodeAsync(r.ClientCode);
                if (orgData != null)
                {
                    requirementResponse.ClientName = orgData.ClientName;
                    requirementResponse.ClientLogo = orgData.LogoURL;
                }

                requirementsResponseList.Add(requirementResponse);
            }

            return new PaginationDto<RequirementResponse>
            {
                Count = totalRecords,
                Page = request.Page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                List = requirementsResponseList
            };
        }
    }
}
