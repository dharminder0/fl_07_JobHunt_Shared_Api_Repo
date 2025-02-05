namespace VendersCloud.Business.Entities.DataModels
{
    public class EmailMessage
    {
        public string FromEmailAddress { get; set; }
        public string FromEmailPassword { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string BrandedEmail { get; set; }
        public bool IsNoReplyMessage { get; set; }

    }
}
