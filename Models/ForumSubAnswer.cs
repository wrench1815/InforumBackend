using System.ComponentModel.DataAnnotations.Schema;
using InforumBackend.Authentication;

namespace InforumBackend.Models
{
    public class ForumSubAnswer
    {
        public long Id { get; set; }

        public string Answer { get; set; }

        public DateTime DatePosted { get; set; }

        // QueryAnswerId as a Relational Field
        [ForeignKey("QueryAnswer")]
        public long QueryAnswerId { get; set; }

        // UserId as a Relational Field
        [ForeignKey("User")]
        public string UserId { get; set; }


        // Navigation Properties

        // Declared virtual Property for lazy loading Related Data
        public virtual ForumAnswer QueryAnswer { get; set; }

        // Declared virtual Property for lazy loading Related Data
        public virtual ApplicationUser User { get; set; }

        public ForumSubAnswer()
        {
            DatePosted = DateTime.Now;
        }
    }
}
