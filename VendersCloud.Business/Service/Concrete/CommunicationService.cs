using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Concrete
{
    public class CommunicationService : ExternalServiceBase
    {
        public IConfiguration _configuration;
        private readonly ExternalConfigReader _externalConfig ;
        public CommunicationService(IConfiguration configuration) : base(configuration[""], configuration[""])
        {
            _configuration = configuration;
            _externalConfig = new ExternalConfigReader(configuration);
        }

        public async Task<bool> SendUserVerificationEmail(string firstname, string lastname, string email, string verificationOtp, string usertoken)
        {
            var url = _externalConfig.GetVerifyEmailDomainUrl();
            string fullname = $"{firstname} {lastname}";
            var emailMessage = new EmailMessage
            {
                To = email,
                Subject = "🔐 Verify Your Email - Welcome to VendorsCloud!",
                Body = $@"
<html>
<body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333;"">
    <div style=""text-align: center; margin: 0 auto; width: 80%;"">
        <h2 style=""color: #2C3E50; background-color: #f3f3f3; padding: 15px; border-radius: 5px;"">Welcome to VendorsCloud, {fullname}! 🎉</h2>
        <p>We're thrilled to have you on board. To complete your registration, please verify your email address.</p>
        
        <div style=""background: #3498db; color: #fff; padding: 20px; border-radius: 5px; margin-top: 20px;"">
            <strong>Your OTP:</strong> <span style=""font-size: 24px; font-weight: bold;"">{verificationOtp}</span>
        </div>

        <p style=""margin-top: 30px;"">Click the button below to verify your email:</p>

        <a href=""{url}/everify/{usertoken}""
           style=""display: inline-block; padding: 15px 25px; background-color: #e74c3c; color: #ffffff;
                  text-decoration: none; border-radius: 5px; font-size: 18px; font-weight: bold; margin-top: 20px;"">
            ✅ Verify Email
        </a>

        <p style=""margin-top: 20px;"">Or, click <a href=""{url}/everify/{usertoken}/{verificationOtp}"" style=""color: #3498db; font-weight: bold;"">here</a> to verify with OTP pre-filled.</p>

        <p>If the above link doesn't work, copy and paste the following into your browser:</p>
        <code>{url}/everify/{usertoken}</code>

        <hr style=""margin-top: 30px; border: none; border-top: 1px solid #ddd;"" />
        <p style=""font-size: 14px; color: #777;"">Need help? Contact our <a href="""" style=""color: #3498db;"">support team</a>.</p>
        <p><strong>— The VendorsCloud Team</strong></p>
    </div>
</body>
</html>"
            };

            var res = await SendEmailAsync(emailMessage);
            return res;
        }


        private async Task<bool> SendEmailAsync(EmailMessage emailMessage)
        {
            try
            {
                var smtpClientdomain = _externalConfig.GetSmtpServerdomain();
                var smtpUser = _externalConfig.GetSmptUserName();
                var smtpPassword = _externalConfig.GetSmtpUserPassword();
                var smtpEmailDomain = _externalConfig.GetSmtpEmailDomain();
                var smtpClient = new SmtpClient(smtpClientdomain)//smtp server domain
                {
                    Port = 587,
                    Credentials = new NetworkCredential(smtpUser, smtpPassword),//username and password 
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpEmailDomain),//email domain
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
