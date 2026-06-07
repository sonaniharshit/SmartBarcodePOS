using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SmartBarcodePOS_Pro.Models;

namespace SmartBarcodePOS_Pro.Services;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public SmtpEmailService(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_settings.FromEmail, _settings.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);

        using var client = new SmtpClient(_settings.Host)
        {
            Port = _settings.Port,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(
                _settings.UserName,
                _settings.Password
            ),
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        await client.SendMailAsync(message);
    }

}