namespace VendersCloud.Data.Repositories.Concrete
{
    public class UsersRepository : StaticBaseRepository<Users>, IUsersRepository
    {
        private readonly IUserProfilesRepository _userProfilesRepository;
        public UsersRepository(IConfiguration configuration, IUserProfilesRepository userProfilesRepository) : base(configuration)
        {
            _userProfilesRepository = userProfilesRepository;
        }
        public async Task<string> InsertUserAsync(RegistrationRequest request, string hashedPassword, byte[] salt, string orgCode,string verificationOtp,string token,string phone)
        {
                var dbInstance = GetDbInstance();
                var tableName = new Table<Users>();

                // Check if the organization exists in USERS table
                var query = new Query(tableName.TableName)
                    .Where("OrgCode", orgCode)
                    .Where("Username", request.Email)
                    .Where("IsDeleted", false)
                    .Select("Id")
                    .Select("OrgCode");

                var existingOrgCode = await dbInstance.ExecuteScalarAsync<string>(query);

                if (!string.IsNullOrEmpty(existingOrgCode))
                {
                var updateQuery = new Query(tableName.TableName).AsUpdate(new
                {
                    Phone= phone,
                    FirstName= request.FirstName,
                    LastName= request.LastName
                }).Where("Username",request.Email);

                await dbInstance.ExecuteScalarAsync<string>(updateQuery);
                string res = "User Already Exists And Details Are Updated!!";
                    return res;
                }

            // Insert new user
            DateTime? lastLoginTime = null;
            var insertQuery = new Query(tableName.TableName).AsInsert(new
                {
                    Phone= phone,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    OrgCode = orgCode,
                    Password = hashedPassword,
                    PasswordSalt = salt,
                    Username = request.Email,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    LastLoginTime = lastLoginTime,
                    IsDeleted = false,
                    VerificationToken = verificationOtp,
                    Token=token
                });

                var insertedUserId = await dbInstance.ExecuteScalarAsync<string>(insertQuery);

                // Re-fetch to ensure insertion
                var query2 = new Query(tableName.TableName).Where("Username", request.Email).Where("OrgCode", orgCode)
                    .Select("Id");

                var insertedOrgCode = await dbInstance.ExecuteScalarAsync<string>(query2);

                return !string.IsNullOrEmpty(insertedOrgCode) ? insertedOrgCode : insertedUserId;
            
        }

        public async Task<Users> GetUserByEmailAsync(string email)
        {
            
                return await GetByAsync(new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate> {
                        Predicates.Field<Users>(f=>f.UserName,Operator.Eq,email),
                        Predicates.Field<Users>(f=>f.IsDeleted,Operator.Eq,false),
                    }
                });
            
        }
        public async Task<bool> UpdateUserTokenAsync(int userId, string token)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Users>();

            var updateQuery = new Query(tableName.TableName)
                .AsUpdate(new
                {
                    Token = token,
                    UpdatedOn = DateTime.UtcNow
                })
                .Where("Id", userId);

            await dbInstance.ExecuteAsync(updateQuery);
            return true;
        }



        public async Task<Users> GetUserByEmailAndOrgCodeAsync(string email,string orgCode)
        {

            return await GetByAsync(new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate> {
                        Predicates.Field<Users>(f=>f.UserName,Operator.Eq,email),
                        Predicates.Field<Users>(f=>f.OrgCode,Operator.Eq,orgCode),
                        Predicates.Field<Users>(f=>f.IsDeleted,Operator.Eq,false),
                    }
            });

        }

        public async Task<bool> DeleteUserByEmailAndOrgCodeAsync(string email, string organizationCode)
        {
                var dbInstance = GetDbInstance();
                var tableName = new Table<Users>();
                var updateQuery = new Query(tableName.TableName)
                        .AsUpdate(new
                        {
                            IsDeleted = true,
                            UpdatedOn = DateTime.UtcNow,
                            LastLoginTime = DateTime.UtcNow
                        })
                        .Where("UserName", email)
                        .Where("OrgCode", organizationCode);

                await dbInstance.ExecuteAsync(updateQuery);
                return true;
            
            
        }

        public async Task<List<Users>> GetAllUserAsync()
        {
            
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM Users";

                var users = dbInstance.Select<Users>(sql).ToList();
                return users;
            
          
        }

        public async Task<List<Users>> GetUserByOrgCodeAsync(string orgCode)
        {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM Users Where OrgCode=@orgCode and isDeleted<>1";

                var users = dbInstance.Select<Users>(sql, new {orgCode}).ToList();
                return users;
            

        }

        public async Task<Users> GetUserByIdAsync(int Id)
        {
              return await GetByAsync(new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate> {
                        Predicates.Field<Users>(f=>f.Id,Operator.Eq,Id),
                        Predicates.Field<Users>(f=>f.IsDeleted,Operator.Eq,false),
                    }
                });
            
        }

        public async Task<bool> VerifyUserEmailAsync(string userToken, string Otp)
        {
            
                var res= await GetByAsync(new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate> {
                        Predicates.Field<Users>(f=>f.Token,Operator.Eq,userToken),
                        Predicates.Field<Users>(f=>f.VerificationToken,Operator.Eq,Otp),
                        Predicates.Field<Users>(f=>f.IsDeleted,Operator.Eq,false),
                    }
                });
                if(res!=null)
                {
                    var dbInstance = GetDbInstance();
                    var tableName = new Table<Users>();
                    var updateQuery = new Query(tableName.TableName)
                            .AsUpdate(new
                            {
                                Verificationtoken = "",
                                IsVerified = true,
                                UpdatedOn = DateTime.UtcNow,
                                LastLoginTime = DateTime.UtcNow
                            })
                            .Where("Token", userToken);

                    await dbInstance.ExecuteAsync(updateQuery);
                    return true;
                }
                return false;
            
        }

        public async Task<Users> GetUserByUserTokenAsync(string userToken)
        {

            var res = await GetByAsync(new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate> {
                        Predicates.Field<Users>(f=>f.Token,Operator.Eq,userToken),
                        Predicates.Field<Users>(f=>f.IsDeleted,Operator.Eq,false),
                    }
            });
            return res;

        }
        public async Task<bool> SetUserPasswordAsync(string hashedPassword, byte[] salt,string userToken)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Users>();
            var updateQuery = new Query(tableName.TableName)
                    .AsUpdate(new
                    {
                        Password = hashedPassword,
                        PasswordSalt = salt,
                        isVerified = true,
                        UpdatedOn = DateTime.UtcNow,
                        LastLoginTime = DateTime.UtcNow
                    })
                    .Where("Token", userToken);

            await dbInstance.ExecuteAsync(updateQuery);
            return true;
        }

        public async Task<bool> UpdateOtpAndTokenAsync(string otp,string token,string email)
        {
                var dbInstance = GetDbInstance();
                var tableName = new Table<Users>();
                var updateQuery = new Query(tableName.TableName)
                        .AsUpdate(new
                        {
                            Verificationtoken = otp,
                            Token = token,
                            UpdatedOn = DateTime.UtcNow,
                            LastLoginTime = DateTime.UtcNow
                        })
                        .Where("Username", email);

                await dbInstance.ExecuteAsync(updateQuery);
                return true;
        }

        public async Task<bool> UpdateUserProfileAsync(UpdateUserProfileRequest request,string uploadedimageUrl)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Users>();
            var insertQuery = new Query(tableName.TableName)
                .AsUpdate(new
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender = request.Gender,
                    Phone = request.Phone,
                    DOB = request.DOB,
                    ProfileAvatar= uploadedimageUrl,
                    IsDeleted = false
                })
                .Where("Username", request.Email);
            await dbInstance.ExecuteAsync(insertQuery);
            return true;
        }

        public async Task<bool> UpdateChangePasswordAsync(ChangePasswordRequest request, string hashedPassword, byte[] salt)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Users>();
            var insertQuery = new Query(tableName.TableName)
                .AsUpdate(new
                {
                   Password= hashedPassword,
                   PasswordSalt= salt,
                    IsDeleted = false
                })
                .Where("Username", request.Email);
            await dbInstance.ExecuteAsync(insertQuery);
            return true;
        }

        public async Task<PaginationDto<UsersDto>> SearchMemberDetailsAsync(SearchMemberRequest request)
        {
            using var connection = GetConnection();
            var predicates = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.OrgCode))
            {
                predicates.Add("(o.OrgCode LIKE @OrgCode)");
                parameters.Add("OrgCode", request.OrgCode);
            }
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                predicates.Add("(o.FirstName LIKE @searchText OR o.LastName LIKE @searchText)");
                parameters.Add("searchText", $"%{request.SearchText}%");
            }

            if (request.Status >= 0)
            {
                predicates.Add("(o.isDeleted <> @Status)");
                parameters.Add("Status", request.Status);
            }

            if (request.Access != null && request.Access.Any())
            {
                var rolePlaceholders = string.Join(", ", request.Access.Select((role, index) => $"@Role{index}"));
                predicates.Add($"EXISTS (SELECT 1 FROM UserProfiles op WHERE op.ProfileId IN ({rolePlaceholders}))");

                for (int i = 0; i < request.Access.Count; i++)
                {
                    parameters.Add($"Role{i}", request.Access[i]);
                }
                parameters.Add("IsDeleted", false);
            }

            string whereClause = predicates.Any() ? "WHERE " + string.Join(" AND ", predicates) : "";

            string query = $@"
                SELECT * FROM Users o
                {whereClause}
                ORDER BY o.CreatedOn DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                
                SELECT COUNT(*) FROM Users o
                {whereClause};";

            parameters.Add("offset", (request.Page - 1) * request.PageSize);
            parameters.Add("pageSize", request.PageSize);

            using var multi = await connection.QueryMultipleAsync(query, parameters);
            var userdata = (await multi.ReadAsync<Users>()).ToList();
            int totalRecords = await multi.ReadFirstOrDefaultAsync<int>();
            string Status = string.Empty;
            var UserDto = new List<UsersDto>();
            foreach (var user in userdata)
            {
                var userProfileRole = await _userProfilesRepository.GetProfileRole(user.Id);
                List<string> userProfileRoles = userProfileRole.Select(role => role.ProfileId.ToString()).ToList();
                var userRoleString = userProfileRoles.Select(role => System.Enum.GetName(typeof(RoleType), Convert.ToInt32(role))).ToList();

                if (user.LastLoginTime == DateTime.MinValue)
                {
                    Status = "Invited";
                }
                else if (user.IsDeleted == false)
                {
                    Status = "Active";
                }
                else
                {
                    Status = "InActive";
                }
              

                var res = new UsersDto {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName= user.UserName,
                    OrgCode= user.OrgCode,
                    Gender= user.Gender,
                    IsVerified= user.IsVerified,
                    ProfileAvatar= user.ProfileAvatar,
                    CreatedOn= user.CreatedOn.ToString("dd-MM-yyyy"),
                    UpdatedOn= user.UpdatedOn.ToString("dd-MM-yyyy"),
                    LastLoginTime= user.LastLoginTime.ToString("dd-MM-yyyy"),
                    IsDeleted= user.IsDeleted,
                    DOB= user.DOB.ToString("dd-MM-yyyy"),
                    Phone= user.Phone,
                    Role = userRoleString,
                    Status= Status
                };
                UserDto.Add(res);
            }
            return new PaginationDto<UsersDto>
            {
                Count = totalRecords,
                Page = request.Page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                List = UserDto
            };
        }

        public async Task<bool> UpdateEmailAsync(string OldEmail, string NewEmail,string verificationOtp)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Users>();
            var update = new Query(tableName.TableName)
                .AsUpdate(new
                {
                    Username = NewEmail,
                    UpdatedOn = DateTime.UtcNow,
                    VerificationToken= verificationOtp,
                    IsVerified= false,
                    IsDeleted = false
                })
                .Where("Username", OldEmail);
            await dbInstance.ExecuteAsync(update);
            return true;
        }
        public async Task<bool> DeleteMemberByIdAsync(int  userId)
        {
            using var connection = GetConnection();

            const string checkQuery = "SELECT COUNT(*) FROM Users WHERE Id = @UserId";
            var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { UserId = userId }) > 0;

            if (!exists)
                return false;

            const string deleteQuery = @"
        UPDATE Users 
        SET IsDeleted = 1, UpdatedOn = GETDATE()
        WHERE Id = @UserId";

            var affectedRows = await connection.ExecuteAsync(deleteQuery, new { UserId = userId });
            return affectedRows > 0;
        }
        public async Task<IEnumerable<EmailTemplatesContent>> GetTemplateContentAsync(string templateKey)
        {
            var dbInstance = GetDbInstance();
            var sql = @"select * from EmailTemplatesContent where templateKey=@templateKey";
            return dbInstance.Select<EmailTemplatesContent>(sql, new { templateKey });
        }



    }

}
