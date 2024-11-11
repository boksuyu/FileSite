using FileSite.Data.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Serilog;

namespace FileSite.Services.EmailServicing;

public class EmailService
{
    private readonly IConfiguration _configuration;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendEmail(EmailVM emailVm)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["Email:Address"]));
        email.To.Add(MailboxAddress.Parse(emailVm.to));
        email.Subject= emailVm.subject;
        email.Body = new TextPart(TextFormat.Html){Text = "<a><a>"};

        var stmp = new SmtpClient();
            stmp.Connect(_configuration["Email:Host"], 587, SecureSocketOptions.StartTls);
            stmp.Authenticate(_configuration["Email:Address"], _configuration["Email:Password"]);
            stmp.Send(email);
            stmp.Disconnect(true);
        
        Log.Information("Password recovery Email sent to {@email}",_configuration["Email:Address"]);
    }
}
