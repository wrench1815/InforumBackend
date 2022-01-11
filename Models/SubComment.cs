using InforumBackend.Authentication;

namespace InforumBackend.Models
{
    public class SubComment
    {
        public long id { get; set; }

        public string Description { get; set; }

        public DateTime DatePosted { get; set; }

        // CommentId as a Relational Field
        public long CommentId { get; set; }

        // UserId as a Relational Field
        public string UserId { get; set; }

        // Navigation Properties

        // Declared virtual Property for lazy loading Related Data
        public virtual Comment Comment { get; set; }

        // Declared virtual Property for lazy loading Related Data
        public virtual ApplicationUser User { get; set; }

        public SubComment()
        {
            DatePosted = DateTime.Now;
        }

    }
}
