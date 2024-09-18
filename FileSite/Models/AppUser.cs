using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileSite.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<FileData> Files { get; set; }
    }
}
