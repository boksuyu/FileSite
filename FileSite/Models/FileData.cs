using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using FileSite.Data.Enums;
using Microsoft.EntityFrameworkCore;
namespace FileSite.Models
{
    public class FileData
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string hash { get; set; }
        [Required]
        public FileFileTimeEnum LifeTime { get; set; }
        public long CreationDate { get; set; }
        public long Size { get; set; }

        [ForeignKey("Owner")]
        public string? OwnerId { get; set; }
        public AppUser? Owner { get; set; }


    }
}
