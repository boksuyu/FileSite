using System.ComponentModel.DataAnnotations;
using FileSite.Data.Enums;

namespace FileSite.Data.ViewModels
{
    public class FileDataVM
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public FileFileTimeEnum LifeTime { get; set; }
    }
}
