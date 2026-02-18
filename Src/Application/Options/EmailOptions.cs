using Microsoft.AspNetCore.Builder.Extensions;

namespace Application.Options;

public class EmailOptions
{
    public const string SectionName = "Email";

    public SmtpSettings Smtp { get; set; } = new();
    public SenderSettings Sender { get; set; } = new();
    public string PropertyBaseUrl { get; set; } = string.Empty;

    public string ConfirmationBaseUrl { get; set; } = string.Empty;
    public bool EnableEmailSending { get; set; }

    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class SenderSettings
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}