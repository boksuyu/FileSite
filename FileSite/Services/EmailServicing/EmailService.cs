using System.Security.Cryptography;
using System.Text;
using FileSite.Data.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Serilog;

namespace FileSite.Services.EmailServicing;

public class EmailService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly IConfiguration _configuration;
    private Dictionary<string, AccountRecoveryVM> _availableEmailChanges;
    private string SiteUrl;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _availableEmailChanges = new Dictionary<string, AccountRecoveryVM>();
        SiteUrl = _configuration["SiteUrl"];
    }

    public void SendEmail(EmailVM emailVm)
    {   string? link=GenerateLink(emailVm.to);
        if (link==null)
        {Log.Error("EORRROROROOROROROOROROROR"); return; }
        AccountRecoveryVM newrec = new AccountRecoveryVM(){
              Date = DateTimeOffset.Now.ToUnixTimeSeconds(),
              Link = link,
              Email = emailVm.to,
          };
        _availableEmailChanges.Add(link, newrec);
        emailVm.body=$"Your account recovery link is: {SiteUrl}/Account/NewPassword/{link} <hr/> <small>This is an automated mail. Please do not reply</small>";

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["Email:Address"]));
        email.To.Add(MailboxAddress.Parse(emailVm.to));
        email.Subject = emailVm.subject;
        email.Body = new TextPart(TextFormat.Html) { Text = emailVm.body };

        var stmp = new SmtpClient();
        stmp.Connect(_configuration["Email:Host"], 587, SecureSocketOptions.StartTls);
        stmp.Authenticate(_configuration["Email:Address"], _configuration["Email:Password"]);
        stmp.Send(email);
        stmp.Disconnect(true);
        
        
        Log.Information("Password recovery mail sent to {@email} with link {@link}",emailVm.to,link);
        Console.WriteLine($"KEEEEEEEEEEYS{ _availableEmailChanges.ContainsKey(link)}#################################################################################################################################################");
    }
    public void CleanUsedLink(string link) => _availableEmailChanges.Remove(link);
    
    /// <summary>
    /// Returns Email of an avaible link if it exists, returns NULL otherwise.
    /// </summary>
    /// <param name="link"></param>
    /// <returns></returns>
    public string? CheckAvailableLink(string link) => _availableEmailChanges[link].Email;
    private void CleanUnusedLinks(object? state)
    {
        foreach (var pair in _availableEmailChanges)
        {
            if (pair.Value.Date+300 < DateTimeOffset.Now.ToUnixTimeSeconds())
            { _availableEmailChanges.Remove(pair.Key); }
        }
    }
    
    /// <summary>
    /// Generates a MD5 code based on the seed and the current UTC
    /// </summary>
    /// <param name="seed">seed</param>
    /// <returns>a MD5 code / null depending on success</returns>
    private string? GenerateLink(string seed)
    {
        string? generated =
            BitConverter.ToString(MD5.Create()
                .ComputeHash(Encoding.UTF8.GetBytes($"{seed}{DateTimeOffset.Now.ToString()}"))); 
        if (generated == null)
        {Log.Error("password recovery link could not be generated.");
            return null;
        }
        
        return generated;
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        Log.Information("Email service started");
        _timer= new Timer(CleanUnusedLinks, null, TimeSpan.Zero, TimeSpan.FromSeconds(20));
        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Log.Information("Email service stopped");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
    public void Dispose() { _timer?.Dispose(); }
}
