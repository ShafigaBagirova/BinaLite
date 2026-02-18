using Application.Abstracts.Services;
using Application.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Infrastructure.Services;

public sealed class SmtpEmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;

    public SmtpEmailService(IOptions<EmailOptions> options)
    {
        _emailOptions = options.Value;
    }

    public async Task SendAsync(
        string toEmail,
        string subject,
        string htmlBody,
        string? plainTextBody = null,
        CancellationToken cancellationToken = default)
    {
        if (!_emailOptions.EnableEmailSending) return;

        if (string.IsNullOrWhiteSpace(toEmail)) return;

        using var message = new MailMessage
        {
            From = new MailAddress(_emailOptions.Sender.Email, _emailOptions.Sender.Name),
            Subject = subject ?? string.Empty,
            Body = htmlBody ?? string.Empty,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);

       
        using var client = new SmtpClient(_emailOptions.Smtp.Host, _emailOptions.Smtp.Port)
        {
            EnableSsl = _emailOptions.Smtp.EnableSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_emailOptions.Smtp.UserName, _emailOptions.Smtp.Password)
        };

        try
        {
            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("SMTP ERROR:");
            Console.WriteLine(ex.ToString());   
            throw;
        }
    }
}
