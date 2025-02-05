using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Concrete
{
    public class CommunicationService : ExternalServiceBase
    {
        public IConfiguration _configuration;

        public CommunicationService(IConfiguration configuration)
    : base(configuration["CommunicationApiUrl"], configuration["CommunicationApiAuthorizationBearer"])
        {
            _configuration = configuration;
        }


        public async Task<bool> SendUserVerificationEmail(string firstname, string lastname, string email, string verificationOtp, string usertoken)
        {
            string fullname = string.Concat(firstname, " ", lastname);
            var emailMessage = new EmailMessage
            {
                To = email,
                Subject = "VendorsCloud: Email Verification Mail",
                Body = $@"Hi {fullname},<br>
                 <p>
                  Welcome to VendorsCloud!
                  Complete your signup by verifying your email<br/>
                  Click on the link below and enter your OTP<br/>
                  Your OTP is {verificationOtp} <br/>
                 </p>
                 <a href=""{_configuration["AccountsCoreUIUrl"]}/everify/{usertoken}"">Verify email</a>
                 <br /><br />
                 <a href=""{_configuration["AccountsCoreUIUrl"]}everify/{usertoken}/{verificationOtp}"">Verify email OTP prefilled</a>
                 <p>
                 VendorsCloud team
                 </p>"
            };

            bool isEmailSent = await SendEmailAsync(emailMessage);
            return isEmailSent;
        }

        private async Task<bool> SendEmailAsync(EmailMessage emailMessage)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.yourserver.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("yourusername", "yourpassword"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("your-email@domain.com"),
                    Subject = emailMessage.Subject,
                    Body = emailMessage.Body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(emailMessage.To);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

    }
}
