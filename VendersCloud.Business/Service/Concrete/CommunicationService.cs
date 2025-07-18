﻿using System.Net;
using System.Net.Mail;
using VendersCloud.Business.CommonMethods;
using VendersCloud.Business.Service.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class CommunicationService : ExternalServiceBase
    {
        public IConfiguration _configuration;
        private readonly ExternalConfigReader _externalConfig ;
        private readonly IUsersRepository _usersRepository;
        public CommunicationService(IUsersRepository usersRepositor,IConfiguration configuration) : base(configuration[""], configuration[""])
        {
            _configuration = configuration;
            _usersRepository = usersRepositor;
            _externalConfig = new ExternalConfigReader(configuration);
        }

        public async Task<bool> SendUserVerificationEmail(string firstname, string lastname, string email, string verificationOtp, string usertoken)
        {
            var url = _externalConfig.GetVerifyEmailDomainUrl();

            // DB se template content load karo (VerificationEmail template)
            var templateContent = await GetTemplateContentAsync("VerificationEmail");

            var emailBody = VCEmailTemplates.GetVerificationEmailTemplate(templateContent, firstname, lastname, verificationOtp, usertoken, url);

            var emailMessage = new EmailMessage
            {
                To = email,
                Subject = templateContent.ContainsKey("Subject") ? templateContent["Subject"] : "🔐 Verify Your Email - Welcome to VendorsCloud!",
                Body = emailBody
            };

            return await SendEmailAsync(emailMessage);
        }

        public async Task<bool> SendUserEmailVerification(string firstname, string lastname, string email, string usertoken)
        {
            var url = _externalConfig.GetVerifyEmailDomainUrl();

            // DB se UserVerificationEmail template content lo
            var templateContent = await GetTemplateContentAsync("UserVerificationEmail");

            var emailBody = VCEmailTemplates.GetUserVerificationEmailTemplate(templateContent, firstname, lastname, usertoken, url);

            var emailMessage = new EmailMessage
            {
                To = email,
                Subject = templateContent.ContainsKey("Subject") ? templateContent["Subject"] : "🔐 Verify Your Email - Welcome to VendorsCloud!",
                Body = emailBody
            };

            return await SendEmailAsync(emailMessage);
        }
        public async Task<bool> SendUserForgotPasswordEmail(string firstname, string lastname, string email, string usertoken)
        {


            var url = _externalConfig.GetVerifyEmailDomainUrl();
            var templateContent = await GetTemplateContentAsync("ForgetPassword");

            var emailBody = GetForgotPasswordEmailTemplate(templateContent, firstname, lastname, usertoken,url);

            var emailMessage = new EmailMessage
            {
                To = email,
                Subject = templateContent.ContainsKey("Title") ? templateContent["Title"] : "🔐 Reset Your Password - VendorsCloud",
                Body = emailBody
            };

            return await SendEmailAsync(emailMessage);
        }

        public static string GetForgotPasswordEmailTemplate(
    Dictionary<string, string> content,
    string firstname,
    string lastname,
    string token,
    string urlBase)
        {
             urlBase = "https://fl-01-ymen-shared-ui-cin-test.azurewebsites.net/setpassword";
            var fullName = $"{firstname} {lastname}";
            var resetLink = $"{urlBase}/{token}";

            return $@"
        <h2>{content["Title"]}</h2>
        <p>Hi {fullName},</p>
        <p>{content["Body"]}</p>
        <p><a href='{resetLink}' style='background:#3B82F6;color:#fff;padding:10px 20px;border-radius:5px;text-decoration:none;'>{content["ButtonText"]}</a></p>
        <p>{content["CopyPasteInstruction"]}</p>
        <p>{resetLink}</p>
        <p>{content["ExpiryNote"]}</p>
        <br/>
        <p>{content["FooterNote"]}</p>
        <p>{content["FooterSignature"]}</p>
    ";
        }

        public async Task<bool> DispatchedInvitationMailAsync(string receiverOrgName, string senderOrgName, string senderEmail, string receiverEmail, string senderMessage)
        {
            
            var templateContent = await GetTemplateContentAsync("InvitationEmail");

            var emailBody = VCEmailTemplates.GetInvitationEmailTemplate(templateContent, receiverOrgName, senderOrgName, senderMessage);

            var emailMessage = new EmailMessage
            {
                To = receiverEmail,
                Subject = templateContent.ContainsKey("Subject") ? templateContent["Subject"] : $"📩 Invitation from {senderOrgName}",
                Body = emailBody
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
        public async Task<Dictionary<string, string>> GetTemplateContentAsync(string templateKey) { 
       
            var templateEntries = await _usersRepository.GetTemplateContentAsync(templateKey);

            return templateEntries.ToDictionary(x => x.ContentKey, x => x.ContentValue);
        }
       

    }
}
