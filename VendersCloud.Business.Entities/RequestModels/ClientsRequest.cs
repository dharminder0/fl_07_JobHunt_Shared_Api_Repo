namespace VendersCloud.Business.Entities.RequestModels
{
    public class ClientsRequest
    {
        public string OrgCode { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }

        public List<FileRequest> LogoURL { get; set; } // Uploaded image file

        public List<FileRequest> FaviconURL { get; set; }  // URL-based favicon

        public int Status { get; set; }
        public int UserId { get; set; }
    }
    public class FileRequest
    {
        public string FileName { get; set; }
        public string FileData { get; set; }
    }
}
