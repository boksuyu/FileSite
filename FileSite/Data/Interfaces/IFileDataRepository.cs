using FileSite.Data.ViewModels;
using FileSite.Models;


namespace FileSite.Data.Interfaces
{
    public interface IFileDataRepository
    {
        Task<string> Add(FileDataVM FileView, string Path);
        void SaveChanges();
        Task<bool> ValidatDistinct(string hash);
        Task<FileData> RequestFileData(string hash);
        //public bool delete(FileData file); //for later timed files
    }
}
