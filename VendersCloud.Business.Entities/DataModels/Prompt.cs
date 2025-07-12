namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "Prompts")]
    public class Prompts : IEntityKey
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string PromptDetail { get; set; }
        public string SystemRole { get; set; }
        public string Model { get; set; }
        public int MaxToken { get; set; }
        public decimal Temperature { get; set; }
        public decimal TopP { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class PromptMapper : ClassMapper<Prompts>
    {
        public PromptMapper()
        {
            Table("Prompt");
            AutoMap();
        }
    }
}
