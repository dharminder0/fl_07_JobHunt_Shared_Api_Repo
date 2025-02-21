using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class OrgRelationshipsRepository : StaticBaseRepository<OrgRelationships>, IOrgRelationshipsRepository
    {
        public OrgRelationshipsRepository(IConfiguration configuration):base(configuration)
        {

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
                });
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
            predicates.Add("r.OrgCode = @orgCode");
            parameters.Add("orgCode", request.OrgCode);

            string whereClause = predicates.Any() ? "WHERE " + string.Join(" AND ", predicates) : "";

            string query = $@"
        SELECT * FROM OrgRelationships r
        {whereClause}
        ORDER BY r.CreatedOn DESC
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

        SELECT COUNT(*) FROM OrgRelationships r {whereClause};";

            parameters.Add("offset", (request.Page - 1) * request.PageSize);
            parameters.Add("pageSize", request.PageSize);

            using var multi = await connection.QueryMultipleAsync(query, parameters);
            var relationships = (await multi.ReadAsync<OrgRelationships>()).ToList();
            int totalRecords = await multi.ReadFirstOrDefaultAsync<int>();

            return new PaginationDto<OrgRelationshipSearchResponse>
            {
                Count = totalRecords,
                Page = request.Page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                List = relationships.Select(item => new OrgRelationshipSearchResponse
                {
                    Id = item.Id,
                    OrgCode = item.OrgCode,
                    RelatedOrgCode = item.RelatedOrgCode,
                    RelationshipType = item.RelationshipType,
                    StatusName = System.Enum.GetName(typeof(InviteStatus), item.Status),
                    Status = item.Status,
                    CreatedBy = item.CreatedBy,
                    UpdatedBy = item.UpdatedBy,
                    CreatedOn = item.CreatedOn,
                    UpdatedOn = item.UpdatedOn,
                    IsDeleted = item.IsDeleted
                }).ToList()
            };
        }

    }
}
 