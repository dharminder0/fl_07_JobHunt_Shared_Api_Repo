namespace VendersCloud.Data.Repositories.Concrete
{
    public class ClientsRepository : StaticBaseRepository<Clients>, IClientsRepository
    {
        public ClientsRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public async Task<bool> UpsertClientAsync(ClientsRequest request, string clientCode, string uploadedimageUrl, string uploadedUrl)
        {
            var dbInstance = GetDbInstance();
            var table = new Table<Clients>();

            // Check if the client already exists
            var query = new Query(table.TableName)
                        .Where("ClientName", request.ClientName)
                        .Where("OrgCode", request.OrgCode)
                        .Select("Id");

            var existingOrgCode = await dbInstance.ExecuteScalarAsync<string>(query);

           

            if (!string.IsNullOrEmpty(existingOrgCode))
            {
                // Update existing client record
                var updateQuery = new Query(table.TableName).AsUpdate(new
                {
                    Description = request.Description,
                    ContactPhone = request.ContactPhone,
                    ContactEmail = request.ContactEmail,
                    Address = request.Address,
                    Website = request.Website,
                    LogoURL = uploadedimageUrl,  
                    FaviconURL = uploadedUrl, 
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = request.UserId,
                    Status = request.Status,
                    isDeleted = false
                }).Where("ClientName", request.ClientName);

                await dbInstance.ExecuteScalarAsync<string>(updateQuery);
                return true;
            }

            // Insert new client record
            var insertQuery = new Query(table.TableName).AsInsert(new
            {
                ClientCode = clientCode,
                OrgCode = request.OrgCode,
                ClientName = request.ClientName,
                Description = request.Description,
                ContactPhone = request.ContactPhone,
                ContactEmail = request.ContactEmail,
                Address = request.Address,
                Website = request.Website,
                LogoURL = uploadedimageUrl,
                FaviconURL = uploadedUrl,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = request.UserId,
                Status = request.Status,
                isDeleted = false
            });

            await dbInstance.ExecuteScalarAsync<string>(insertQuery);
            return true;
        }


        public async Task<Clients> GetClientsByIdAsync(int id)
        {

            return await GetByAsync(new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate> {
                        Predicates.Field<Clients>(f=>f.Id,Operator.Eq,id),
                        Predicates.Field<Clients>(f=>f.IsDeleted,Operator.Eq,false),
                    }
            });

        }

        public async Task<Clients> GetClientsByNameAsync(string name)
        {

            return await GetByAsync(new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate> {
                        Predicates.Field<Clients>(f=>f.ClientName,Operator.Eq,name),
                        Predicates.Field<Clients>(f=>f.IsDeleted,Operator.Eq,false),
                    }
            });

        }
        public async Task<List<Clients>> GetClientsByOrgCodeAsync(string orgCode)
        {

            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Clients Where OrgCode=@orgCode And isDeleted=0";

            var clients = dbInstance.Select<Clients>(sql, new { orgCode }).ToList();
            return clients;

        }
        public async Task<Clients> GetClientsByClientCodeAsync(string clientCode)
        {

            return await GetByAsync(new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate> {
                        Predicates.Field<Clients>(f=>f.ClientCode,Operator.Eq,clientCode),
                        Predicates.Field<Clients>(f=>f.IsDeleted,Operator.Eq,false),
                    }
            });

        }
        public async Task<List<Clients>> GetClientsByClientCodeListAsync(List<string> clientCode)
        {

            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Clients Where ClientCode In @clientCode And isDeleted=0";

            var clients = dbInstance.Select<Clients>(sql, new { clientCode }).ToList();
            return clients;

        }
        public async Task<PaginationDto<ClientsResponse>> GetClientsListAsync(ClientsSearchRequest request)
        {
            using var connection = GetConnection();
            var predicates = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(request.searchText))
            {
                predicates.Add("(c.ClientName LIKE @searchText OR c.ClientCode LIKE @searchText)");
                parameters.Add("searchText", $"%{request.searchText}%");
            }

            if (request.Status !=null && request.Status.Any())
            {
                predicates.Add("(c.Status in  @status)");
                parameters.Add("status", request.Status);
            }

            predicates.Add("c.isDeleted = 0");
            predicates.Add("c.OrgCode = @orgCode");
            parameters.Add("orgCode", request.OrgCode);

            string whereClause = predicates.Any() ? "WHERE " + string.Join(" AND ", predicates) : "";

            string query = $@"
    SELECT * FROM Clients c
    {whereClause}
    ORDER BY c.CreatedOn DESC
    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

    SELECT COUNT(*) FROM Clients c {whereClause};";

            parameters.Add("offset", (request.page - 1) * request.pageSize);
            parameters.Add("pageSize", request.pageSize);

            using var multi = await connection.QueryMultipleAsync(query, parameters);
            var clients = (await multi.ReadAsync<Clients>()).ToList();
            int totalRecords = await multi.ReadFirstOrDefaultAsync<int>();

            var clientsResponseList = clients.Select(c => new ClientsResponse
            {
                Id = c.Id,
                ClientCode = c.ClientCode,
                OrgCode = c.OrgCode,
                ClientName = c.ClientName,
                Description = c.Description,
                ContactPhone = c.ContactPhone,
                ContactEmail = c.ContactEmail,
                Address = c.Address,
                Website = c.Website,
                LogoURL = c.LogoURL,
                FaviconURL = c.FaviconURL,
                CreatedOn = c.CreatedOn,
                UpdatedOn = c.UpdatedOn,
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy,
                Status = c.Status,
                StatusName = System.Enum.GetName(typeof(ClientStatus), c.Status),
                IsDeleted = c.IsDeleted
            }).ToList();

            return new PaginationDto<ClientsResponse>
            {
                Count = totalRecords,
                Page = request.page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)request.pageSize),
                List = clientsResponseList
            };
        }



        public async Task<bool> DeleteClientsByIdAsync(string orgCode, int id, string clientName)
        {   
            var dbInstance = GetDbInstance();
            var table = new Table<Clients>();
            var query = new Query(table.TableName)
                        .Where("ClientName", clientName)
                        .Where("OrgCode", orgCode)
                        .Select("Id");
            var existingOrgCode = await dbInstance.ExecuteScalarAsync<string>(query);

            if (!string.IsNullOrEmpty(existingOrgCode))
            {
                var updateQuery = new Query(table.TableName).AsUpdate(new
                {
                    UpdatedOn = DateTime.UtcNow,
                    isDeleted = true
                }).Where("ClientName", clientName);

                await dbInstance.ExecuteScalarAsync<string>(updateQuery);
                return true;
            }
            return false;
        }
    }
}

