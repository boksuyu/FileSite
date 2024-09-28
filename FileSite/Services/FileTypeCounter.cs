using FileSite.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Nodes;

namespace FileSite.Services
{   //cant add this is as a service of ANY KIND, have to use it inside a scoped one like a normal class
    public class FileTypeCounter : IHostedService, IDisposable
    {
        public Dictionary<string, int> Dict = new Dictionary<string, int>();
        private Timer? _timer;

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
        {
            _timer = new(CountFileExtensions, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() { _timer?.Dispose(); }
    }
}
