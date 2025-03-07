namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "PromptExecutionLog")]
    public class PromptExecutionLog :IEntityKey
    {
        public int Id { get; set; } 
        public string PromptCode { get; set; } 
        public string Request { get; set; }  
        public string Response { get; set; } 
        public int  CreatedBy { get; set; }  
        public DateTime CreatedOn { get; set; } 
    }
    public class PromptExecutionLogMapper : ClassMapper<PromptExecutionLog>
    {
        public PromptExecutionLogMapper()
        {
            Table("PromptExecutionLog");
            AutoMap();
        }
    }
   

}
