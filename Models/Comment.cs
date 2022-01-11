using System.ComponentModel.DataAnnotations.Schema;
using InforumBackend.Authentication;

namespace InforumBackend.Models
{
    public class Comment
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public DateTime DatePosted { get; set; }

        // UserId as a Relational Field
        [ForeignKey("User")]
        public string UserId { get; set; }

        // PostId as a Relational Field
        [ForeignKey("Post")]
        public long PostId { get; set; }

        // Navigation Properties

        // Declared virtual User Property for lazy loading Related Data
        public virtual ApplicationUser User { get; set; }

        // Declared virtual Post Property for lazy loading Related Data
        public virtual BlogPost Post { get; set; }

        public Comment()
        {
            DatePosted = DateTime.Now;
        }
    }
}
