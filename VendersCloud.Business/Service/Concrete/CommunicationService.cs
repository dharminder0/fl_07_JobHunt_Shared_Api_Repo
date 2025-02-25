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
                        
                        <div style=""display: inline-block;
                                  text-decoration: none; border-radius: 5px; font-size: 18px; font-weight: bold; margin-top: 20px;"">
                            <strong>Your OTP:</strong> <span style=""font-size: 18px; font-weight: bold;color:#4640DE"">{verificationOtp}</span>
                        </div>
                
                        <p style=""margin-top: 30px;"">Click the button below to verify your email:</p>
                
                        <a href=""{url}/everify/{usertoken}"" style=""display: inline-block; padding: 8px 25px; background-color: #4640DE; color: #ffffff;
                                  text-decoration: none; border-radius: 5px; font-size: 18px; font-weight: 600; margin-top: 20px;"">
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

        public async Task<bool> SendUserEmailVerification(string firstname, string lastname, string email, string usertoken)
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
                        <p>We're thrilled to have you on board.Please set your credentials.</p>
                        
                
                        <p style=""margin-top: 30px;"">Click the button below to set your credentials:</p>
                
                        <a href=""{url}/setpassword/{usertoken}"" style=""display: inline-block; padding: 8px 25px; background-color: #4640DE; color: #ffffff;
                                  text-decoration: none; border-radius: 5px; font-size: 18px; font-weight: 600; margin-top: 20px;"">
                            ✅ Set Credentials
                        </a>
                        <p>If the above link doesn't work, copy and paste the following into your browser:</p>
                        <code>{url}/setpassword/{usertoken}</code>
                
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
        public async Task<bool> DispatchedInvitationMailAsync(string receiverOrgName,string senderOrgName, string senderEmail ,string receiverEmail,  string senderMessage)
        {
            var emailMessage = new EmailMessage
            {
                To = receiverEmail,
                Subject = $"📩 Invitation from {senderOrgName}",
                Body = $@"
                <html>
                <body style=""font-family: Arial, sans-serif; color: #2C3E50; background-color: #f3f3f3; padding: 20px;"">
                    <div style=""max-width: 600px; margin: auto; background: #ffffff; padding: 20px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);"">
                        <h2 style=""color: #4640DE; text-align: center;"">📩 Invitation from {senderOrgName}</h2>
                        <hr style=""border: none; border-top: 2px solid #eee; margin: 10px 0;"">
                        
                        <p style=""font-size: 16px; color: #555;""><strong>Hello {receiverOrgName} Team,</strong></p>
                        <p style=""font-size: 15px; color: #555;"">You have received an invitation from <strong>{senderOrgName}</strong>.</p>
                
                        <div style=""background-color: #f3f3f3; padding: 15px; border-radius: 5px; font-size: 14px; color: #333;"">
                            <strong>Message from {senderOrgName}:</strong><br/>
                            <i>{(string.IsNullOrWhiteSpace(senderMessage) ? "We would like to invite you to collaborate with us." : senderMessage)}</i>
                        </div>
                
                        <p style=""margin-top: 20px; font-size: 14px; color: #555;"">Looking forward to your response.</p>
                
                        <hr style=""border: none; border-top: 1px solid #ddd; margin: 20px 0;"">
                        <p style=""text-align: center; font-size: 13px; color: #777;"">
                            Best Regards, <br> <strong>{senderOrgName} Team</strong>
                        </p>
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
                var smtpClient = new SmtpClient(smtpClientdomain)//smtp server domain
                {
                    Port = 587,
                    Credentials = new NetworkCredential(smtpUser, smtpPassword),//username and password 
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser),//email domain
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
