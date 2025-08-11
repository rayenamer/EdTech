using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace API.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpSettings> settings, ILogger<SmtpEmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        
        // Debug logging of SMTP settings (excluding password)
        _logger.LogInformation("SMTP Settings loaded - Host: {Host}, Port: {Port}, Username: {Username}, FromEmail: {FromEmail}, EnableSsl: {EnableSsl}",
            _settings.Host,
            _settings.Port,
            _settings.Username,
            _settings.FromEmail,
            _settings.EnableSsl);
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            if (string.IsNullOrEmpty(_settings.Username) || string.IsNullOrEmpty(_settings.Password))
            {
                _logger.LogError("SMTP credentials are missing. Username is null/empty: {UsernameEmpty}, Password is null/empty: {PasswordEmpty}",
                    string.IsNullOrEmpty(_settings.Username),
                    string.IsNullOrEmpty(_settings.Password));
                throw new InvalidOperationException("SMTP credentials are not configured. Please check your settings.");
            }

            _logger.LogInformation("Creating SMTP client with settings - Host: {Host}, Port: {Port}, Username: {Username}",
                _settings.Host, _settings.Port, _settings.Username);

            var smtpClient = new SmtpClient
            {
                Host = _settings.Host,
                Port = _settings.Port,
                EnableSsl = _settings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                Timeout = 10000 // 10 seconds timeout
            };

            // Enable detailed SMTP debugging
            smtpClient.SendCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    _logger.LogError("SMTP Send Error: {Error}", e.Error.Message);
                }
                if (e.Cancelled)
                {
                    _logger.LogWarning("SMTP Send was cancelled");
                }
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
                Priority = MailPriority.Normal
            };
            mailMessage.To.Add(email);

            _logger.LogInformation("Attempting to send email to {Email} using SMTP server {Host}:{Port}", 
                email, _settings.Host, _settings.Port);
            
            await smtpClient.SendMailAsync(mailMessage);
            
            _logger.LogInformation("Successfully sent email to {Email}", email);
        }
        catch (SmtpException smtpEx)
        {
            _logger.LogError(smtpEx, "SMTP Error sending email to {Email}. StatusCode: {StatusCode}, Message: {Message}",
                email, smtpEx.StatusCode, smtpEx.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}. SMTP Host: {Host}, Port: {Port}, Username: {Username}",
                email, _settings.Host, _settings.Port, _settings.Username);
            throw;
        }
    }
}

public class SmtpSettings
{
    public required string Host { get; set; }
    public int Port { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string FromEmail { get; set; }
    public required string FromName { get; set; }
    public bool EnableSsl { get; set; }
}