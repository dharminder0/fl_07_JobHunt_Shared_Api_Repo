namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "MatchResults")]
    public class MatchResults : IEntityKey
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public int RequirementId { get; set; }
        public int MatchScore { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class MatchResultsMapper : ClassMapper<MatchResults>
    {
        public MatchResultsMapper()
        {
            Table("MatchResults");
            AutoMap();
        }
    }
}
