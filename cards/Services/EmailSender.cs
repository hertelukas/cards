using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace cards.Services;

public class EmailSender : IEmailSender
{
    private string host;
    private int port;
    private bool enableSSL;
    private string username;
    private string password;

    public EmailSender(string host, int port, bool enableSSL, string username, string password)
    {
        this.host = host;
        this.port = port;
        this.enableSSL = enableSSL;
        this.username = username;
        this.password = password;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = enableSSL
        };
        
        return client.SendMailAsync(
            new MailMessage(username, email, subject, htmlMessage) {IsBodyHtml = true});
    }
}