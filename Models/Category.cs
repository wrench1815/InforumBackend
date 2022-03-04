using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace InforumBackend.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Category
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Slug { get; set; }
    }
}
