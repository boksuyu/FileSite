using FileSite.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Nodes;

namespace FileSite.Services
{
    public class FileTypeCounter : IHostedService, IDisposable
    {
        public Dictionary<string, int> Dict = new Dictionary<string, int>();
        private Timer? _timer;
        private ILogger<FileTypeCounter> _logger;
        public FileTypeCounter(ILogger<FileTypeCounter> logger)
        {
            _logger = logger;
        }

        public void CountFileExtensions(object? state)
        {
            var options = new DbContextOptionsBuilder().UseSqlServer(JsonNode.Parse(File.ReadAllText("appsettings.json"))["ConnectionStrings"]["DefaultConnection"].ToString());
            ApplicationDbContext context = new(options.Options);
            int K = 0;
            Dict.Clear();
            foreach (var i in context.FileDatas)
            {
                FileInfo fileInfo = new(i.Location);
                Dict.TryGetValue(fileInfo.Extension, out K);
                Dict[fileInfo.Extension] = ++K;

            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {   if(_timer != null) { _timer.Dispose(); }
            _logger.LogInformation(eventId:100,"FileTypeCounter is running");
            _timer = new(CountFileExtensions, null, TimeSpan.Zero, TimeSpan.FromHours(6.0));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(eventId:-100,"FileTypeCounter has stopped");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() { _timer?.Dispose(); }
    }
}
