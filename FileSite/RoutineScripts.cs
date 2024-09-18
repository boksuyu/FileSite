using FileSite.Data;
using FileSite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Diagnostics;
using FileSite.Data.Enums;
using Microsoft.Extensions.Configuration;
using System.Timers;
namespace FileSite
{
    public static class RoutineScripts
    {
        public static async Task CheckFileLifeTime(object? sender, string path) {

            var options=new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(path);
            ApplicationDbContext _context = new(options.Options);
            List<FileData> toBeDeleted = await _context.FileDatas.ToListAsync<FileData>();
            Console.WriteLine("HELO!!!!!!!!!!!!!:DDDDDDDDD");
            foreach (FileData fileData in toBeDeleted) 
            {
                switch (fileData.LifeTime) 
                {
                    case FileFileTimeEnum.oneDay:
                        if (fileData.CreationDate + 86400 < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                        {
                            File.Delete(fileData.Location);
                            _context.Remove(fileData);
                        }
                        break;
                    case FileFileTimeEnum.oneWeek:
                        if (fileData.CreationDate + 604800 < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                        {
                            File.Delete(fileData.Location);
                            _context.Remove(fileData); }
                        break;
                    case FileFileTimeEnum.oneMonth:
                        if (fileData.CreationDate + 2629743 < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                        {
                            File.Delete(fileData.Location);
                            _context.Remove(fileData); 
                        }
                        break;
                    case FileFileTimeEnum.oneYear:
                        if (fileData.CreationDate + 31556926 < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                        {
                          _context.Remove(fileData);
                          File.Delete(fileData.Location);
                        }
                        break;
                    case FileFileTimeEnum.Permanent:
                        break;
                }
            }
            _context.SaveChanges();
        }
    }
}
