using FileSite.Data;
using FileSite.Models;
using FileSite.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Runtime.Intrinsics;

namespace FileSite.Repositories
{   /// <summary>
/// This class exists to test singletons
/// </summary>
    public class GlobalDataRepository
    {
        private readonly ApplicationDbContext _context;
        public FileTypeCounter FileTypes;
        public DateTimeOffset LastCheck { get; private set; }
        public long TotalSize { get; private set; }
        public int FileAmount { get; private set; }
        public GlobalDataRepository()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(config["ConnectionStrings:DefaultConnection"]);
             _context = new(options.Options);
            LastCheck = DateTimeOffset.UnixEpoch;
            FileTypes = new FileTypeCounter();
            FileTypes.StartAsync(CancellationToken.None);
            Console.WriteLine("YWNBAW");
        }

        public void UpdateGlobalData()
        {
            //if (LastCheck.ToUnixTimeSeconds() + 120L> DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            //{ return;
            //}

            long temp = 0;
            int amount = 0;
            foreach(var i in _context.FileDatas)
            { temp += i.Size;
              amount++;
            }
            TotalSize = temp;
            FileAmount = amount;
            LastCheck = DateTimeOffset.UtcNow;
            
        }
    }
}
