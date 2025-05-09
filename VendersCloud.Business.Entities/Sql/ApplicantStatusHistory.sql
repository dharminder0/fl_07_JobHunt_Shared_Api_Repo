CREATE TABLE ApplicantStatusHistory (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ApplicantId INT NOT NULL,                -- FK to Applicants table
    Status INT NOT NULL,                     -- RecruitmentStatus enum value
    ChangedBy NVARCHAR(100) NOT NULL,        -- UserId or username
    ChangedOn DATETIME NOT NULL DEFAULT GETDATE(),

);
