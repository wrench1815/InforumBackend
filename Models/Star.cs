using System.ComponentModel.DataAnnotations;

namespace InforumBackend.Models
{
    public class Star
    {
        public long Id { get; set; }

        [Required]
        public long BlogPostId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
