namespace Skeleton.Services.Api.Configurations
{
    public class SendinblueConfig
    {
        public string SmtpName { get; set; }
        public string SmtpEmail { get; set; }
        public string ConfirmEmailTemplateId { get; set; }
        public string AccountCreatedSuccessfullyTemplateId { get; set; }
        public string ResetYourPasswordTemplateId { get; set; }
        public string PasswordResetSuccessfullyTemplateId { get; set; }
    }
}