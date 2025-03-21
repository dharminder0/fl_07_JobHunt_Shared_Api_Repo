using System.Net;
using System.Net.Mail;
using VendersCloud.Business.CommonMethods;

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
            var emailMessage = new EmailMessage
            {
                To = email,
                Subject = "🔐 Verify Your Email - Welcome to VendorsCloud!",
                Body = VCEmailTemplates.GetVerificationEmailTemplate(firstname, lastname, email, verificationOtp, usertoken, url)
            };

            return await SendEmailAsync(emailMessage);
        }

        public async Task<bool> SendUserEmailVerification(string firstname, string lastname, string email, string usertoken)
        {
            var url = _externalConfig.GetVerifyEmailDomainUrl();
            var emailMessage = new EmailMessage
            {
                To = email,
                Subject = "🔐 Verify Your Email - Welcome to VendorsCloud!",
                Body = VCEmailTemplates.GetUserVerificationEmailTemplate(firstname, lastname, usertoken, url)
            };

            return await SendEmailAsync(emailMessage);
        }

        public async Task<bool> DispatchedInvitationMailAsync(string receiverOrgName, string senderOrgName, string senderEmail, string receiverEmail, string senderMessage)
        {
            var emailMessage = new EmailMessage
            {
                To = receiverEmail,
                Subject = $"📩 Invitation from {senderOrgName}",
                Body = VCEmailTemplates.GetInvitationEmailTemplate(receiverOrgName, senderOrgName, senderMessage)
            };

            return await SendEmailAsync(emailMessage);
        }



        private async Task<bool> SendEmailAsync(EmailMessage emailMessage)
        {
            try
            {
                var smtpClientdomain = _externalConfig.GetSmtpServerdomain();
                var smtpUser = _externalConfig.GetSmptUserName();
                var smtpPassword = _externalConfig.GetSmtpUserPassword();
                var smtpClient = new SmtpClient(smtpClientdomain)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(smtpUser, smtpPassword),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser),
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
