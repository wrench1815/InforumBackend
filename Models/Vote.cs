using System.ComponentModel.DataAnnotations;

namespace InforumBackend.Models
{
    public class Vote
    {
        public long Id { get; set; }

        [Required]
        public long ForumId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
