using System.Net;

namespace VendersCloud.Common.Utils
{
    public class FtpManager {
        public static void UploadFile(string fileName, string ftpUploadUrl, string userName, string password, bool useSsl, Stream file) {
            var url = string.Format("{0}/{1}", ftpUploadUrl.Trim('/'), fileName);
            var ftpClient = (FtpWebRequest)FtpWebRequest.Create(url);
            ftpClient.Credentials = new System.Net.NetworkCredential(userName, password);
            ftpClient.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
            ftpClient.UseBinary = true;
            ftpClient.KeepAlive = true;
            ftpClient.EnableSsl = useSsl;
            ftpClient.ContentLength = file.Length;
            var buffer = new byte[4097];
            var totalBytes = (int)file.Length;
            var rs = ftpClient.GetRequestStream();
            while (totalBytes > 0) {
                var bytes = file.Read(buffer, 0, buffer.Length);
                rs.Write(buffer, 0, bytes);
                totalBytes = totalBytes - bytes;
            }
            //fs.Flush();
            file.Close();
            rs.Close();
            var uploadResponse = (FtpWebResponse)ftpClient.GetResponse();
            var responseValue = uploadResponse.StatusDescription;
            uploadResponse.Close();
        }
    }
}
