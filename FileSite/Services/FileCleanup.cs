using FileSite.Data;
using FileSite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Diagnostics;
using FileSite.Data.Enums;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;
using Serilog;

namespace FileSite.Services;

public class FileCleanup : IHostedService, IDisposable
{   //default implementation of logger is still in use
    private ILogger<FileCleanup> _logger;
    private Timer? _timer = null;


    public FileCleanup(ILogger<FileCleanup> logger)
    {
        _logger = logger;
    }

    public async void CheckFileLifeTime(object? state)
    {
        var options = new DbContextOptionsBuilder().UseNpgsql(JsonNode.Parse(File.ReadAllText("appsettings.json"))["ConnectionStrings"]["DefaultConnection"].ToString());
        ApplicationDbContext _context = new(options.Options);


        List<FileData> toBeDeleted = await _context.FileDatas.ToListAsync();
        foreach (FileData fileData in toBeDeleted)
        {
            switch (fileData.LifeTime)
            {
                case FileFileTimeEnum.oneDay:
                    if (fileData.CreationDate + 86400 < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    {
                        File.Delete(fileData.Location);
                        Log.Information("Deleting {@fileData.Location}. Lifetime Ended",fileData.Location);
                        _context.Remove(fileData);
                    }
                    break;
                case FileFileTimeEnum.oneWeek:
                    if (fileData.CreationDate + 604800 < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    {
                        File.Delete(fileData.Location);
                        Log.Information("Deleting {@fileData.Location}. Lifetime Ended",fileData.Location);
                        _context.Remove(fileData);
                    }
                    break;
                case FileFileTimeEnum.oneMonth:
                    if (fileData.CreationDate + 2629743 < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    {
                        File.Delete(fileData.Location);
                        Log.Information("Deleting {@fileData.Location}. Lifetime Ended",fileData.Location);
                        _context.Remove(fileData);
                    }
                    break;
                case FileFileTimeEnum.oneYear:
                    if (fileData.CreationDate + 31556926 < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    {
                        File.Delete(fileData.Location);
                        Log.Information("Deleting {@fileData.Location}. Lifetime Ended",fileData.Location);
                        _context.Remove(fileData);
                    }
                    break;
                case FileFileTimeEnum.Permanent:
                    break;
            }
        }
        _context.SaveChanges();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(eventId:100,"File Cleanup is running.");
        _timer = new Timer(CheckFileLifeTime, null, TimeSpan.Zero, TimeSpan.FromHours(6.0));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(eventId:-100,"File Cleanup has stopped.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
    public void Dispose() { _timer?.Dispose(); }
}
