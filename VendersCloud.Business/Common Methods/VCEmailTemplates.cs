namespace VendersCloud.Business.CommonMethods
{
    public static class VCEmailTemplates
    {
        public static string GetVerificationEmailTemplate(Dictionary<string, string> content, string firstname, string lastname, string verificationOtp, string usertoken, string url)
        {
            string fullname = $"{firstname} {lastname}";

            // Helper function to safely get value from dictionary with fallback default
            string GetValueOrDefault(string key, string defaultValue = "")
            {
                return content.ContainsKey(key) ? content[key] : defaultValue;
            }

            string title = GetValueOrDefault("Title", $"Welcome, {fullname}!");
            title = title.Replace("{fullname}", fullname);

            string body = GetValueOrDefault("Body", "Please verify your email.");
            string otpLabel = GetValueOrDefault("OtpLabel", "Verification Code:");
            string buttonInstruction = GetValueOrDefault("ButtonInstruction", "Click the button below to verify your email:");
            string buttonText = GetValueOrDefault("ButtonText", "Verify Email");
            string otpVerificationLinkInstruction = GetValueOrDefault("OtpVerificationLinkInstruction", "Or verify using this link:");
            string otpVerificationLinkText = GetValueOrDefault("OtpVerificationLinkText", "Verify Link");
            string copyPasteInstruction = GetValueOrDefault("CopyPasteInstruction", "If the button doesn’t work, copy and paste this URL in your browser:");
            string footerNote = GetValueOrDefault("FooterNote", "Thank you for using VendorsCloud.");
            string footerSignature = GetValueOrDefault("FooterSignature", "The VendorsCloud Team");

            return $@"
<html>
<body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333;"">
    <div style=""text-align: center; margin: 0 auto; width: 80%;"">
        <h2 style=""color: #2C3E50; background-color: #f3f3f3; padding: 15px; border-radius: 5px;"">{title}</h2>
        <p>{body}</p>
        
        <div style=""display: inline-block; text-decoration: none; border-radius: 5px; font-size: 18px; font-weight: bold; margin-top: 20px;"">
            <strong>{otpLabel}</strong> <span style=""font-size: 18px; font-weight: bold;color:#4640DE"">{verificationOtp}</span>
        </div>

        <p style=""margin-top: 30px;"">{buttonInstruction}</p>

        <a href=""{url}/everify/{usertoken}"" style=""display: inline-block; padding: 8px 25px; background-color: #4640DE; color: #ffffff;
                  text-decoration: none; border-radius: 5px; font-size: 18px; font-weight: 600; margin-top: 20px;"">
            {buttonText}
        </a>

        <p style=""margin-top: 20px;"">{otpVerificationLinkInstruction} <a href=""{url}/everify/{usertoken}/{verificationOtp}"" style=""color: #3498db; font-weight: bold;"">{otpVerificationLinkText}</a>.</p>

        <p>{copyPasteInstruction}</p>
        <code>{url}/everify/{usertoken}</code>

        <hr style=""margin-top: 30px; border: none; border-top: 1px solid #ddd;"" />
        <p style=""font-size: 14px; color: #777;"">{footerNote}</p>
        <p><strong>{footerSignature}</strong></p>
    </div>
</body>
</html>";
        }


        public static string GetUserVerificationEmailTemplate(
    Dictionary<string, string> content,
    string firstname,
    string lastname,
    string usertoken,
    string url)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content), "Content dictionary cannot be null.");
            }

            // Helper method to safely get content value or default
            string GetContentValue(string key, string defaultValue = "")
            {
                return content.ContainsKey(key) ? content[key] : defaultValue;
            }

            // Build fullname safely
            string fullname = $"{firstname ?? ""} {lastname ?? ""}".Trim();

            // Prepare verification URL
            string verificationUrl = $"{url?.TrimEnd('/')}/setpassword/{usertoken}";

            return $@"
<html>
<body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333;"">
    <div style=""text-align: center; margin: 0 auto; width: 80%;"">
        <h2 style=""color: #2C3E50; background-color: #f3f3f3; padding: 15px; border-radius: 5px;"">
            {GetContentValue("Title").Replace("{fullname}", fullname)}
        </h2>
        <p>{GetContentValue("Body")}</p>

        <p style=""margin-top: 30px;"">{GetContentValue("ButtonInstruction")}</p>

        <a href=""{verificationUrl}"" style=""display: inline-block; padding: 8px 25px; background-color: #4640DE; color: #ffffff;
                  text-decoration: none; border-radius: 5px; font-size: 18px; font-weight: 600; margin-top: 20px;"">
            {GetContentValue("ButtonText", "Verify Email")}
        </a>

        <p>{GetContentValue("CopyPasteInstruction")}</p>
        <code>{verificationUrl}</code>

        <hr style=""margin-top: 30px; border: none; border-top: 1px solid #ddd;"" />
        <p style=""font-size: 14px; color: #777;"">{GetContentValue("FooterNote")}</p>
        <p><strong>{GetContentValue("FooterSignature")}</strong></p>
    </div>
</body>
</html>";
        }



        public static string GetInvitationEmailTemplate(Dictionary<string, string> content, string receiverOrgName, string senderOrgName, string senderMessage)
        {
            string defaultMessage = GetValue(content, "DefaultMessage");
            string formattedMessage = string.IsNullOrWhiteSpace(senderMessage) ? defaultMessage : senderMessage;

            string title = GetValue(content, "Title").Replace("{senderOrgName}", senderOrgName);
            string greeting = GetValue(content, "Greeting").Replace("{receiverOrgName}", receiverOrgName);
            string body = GetValue(content, "Body").Replace("{senderOrgName}", senderOrgName);
            string messageLabel = GetValue(content, "MessageLabel").Replace("{senderOrgName}", senderOrgName);
            string closing = GetValue(content, "Closing");
            string footerSignature = GetValue(content, "FooterSignature").Replace("{senderOrgName}", senderOrgName);

            return $@"
<html>
<body style=""font-family: Arial, sans-serif; color: #2C3E50; background-color: #f3f3f3; padding: 20px;"">
    <div style=""max-width: 600px; margin: auto; background: #ffffff; padding: 20px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);"">
        <h2 style=""color: #4640DE; text-align: center;"">{title}</h2>
        <hr style=""border: none; border-top: 2px solid #eee; margin: 10px 0;"">
        
        <p style=""font-size: 16px; color: #555;""><strong>{greeting}</strong></p>
        <p style=""font-size: 15px; color: #555;"">{body}</p>

        <div style=""background-color: #f3f3f3; padding: 15px; border-radius: 5px; font-size: 14px; color: #333;"">
            <strong>{messageLabel}</strong><br/>
            <i>{formattedMessage}</i>
        </div>

        <p style=""margin-top: 20px; font-size: 14px; color: #555;"">{closing}</p>

        <hr style=""border: none; border-top: 1px solid #ddd; margin: 20px 0;"">
        <p style=""text-align: center; font-size: 13px; color: #777;"">
            {footerSignature}
        </p>
    </div>
</body>
</html>";
        }

        public static string GetValue(Dictionary<string, string> dict, string key)
        {
            return dict.ContainsKey(key) ? dict[key] : string.Empty;
        }
    }
}
