using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "EmailTemplatesContent")]
    public class EmailTemplatesContent : IEntityKey
    {

        public int Id { get; set; }     
        public string TemplateKey { get; set; }  
        public string ContentKey { get; set; }   
        public string ContentValue { get; set; } 
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; } 
    }
    public class EmailTemplatesContentMapper : ClassMapper<EmailTemplatesContent>
    {
        public EmailTemplatesContentMapper()
        {
            Table("EmailTemplatesContent");
            AutoMap();
        }

    }
}
