using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "ApplicantStatusHistory")]
    public class ApplicantStatusHistory : IEntityKey
    {
        public int Id { get; set; }
        public int ApplicantId { get; set; }
        public int Status { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public string  Comment { get; set; }
    }
}
