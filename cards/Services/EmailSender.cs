using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace cards.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;

    private readonly string _host;
    private readonly int _port;
    private readonly bool _enableSsl;
    private readonly string _username;
    private readonly string _password;

    public EmailSender(string host, int port, bool enableSsl, string username, string password)
    {
        _host = host;
        _port = port;
        _enableSsl = enableSsl;
        _username = username;
        _password = password;
        _logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger<EmailSender>();
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        _logger.LogInformation("Sending email {Subject} to {Email}", subject, email);
        var client = new SmtpClient(_host, _port)
        {
            Credentials = new NetworkCredential(_username, _password),
            EnableSsl = _enableSsl
        };

        return client.SendMailAsync(
            new MailMessage(_username, email, subject, htmlMessage) {IsBodyHtml = true});
    }
}