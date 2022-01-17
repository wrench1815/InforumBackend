using System.ComponentModel.DataAnnotations.Schema;
using InforumBackend.Authentication;

namespace InforumBackend.Models
{
    public class ForumAnswer
    {
        public long Id { get; set; }

        // Answer for the Query Posted by a User
        public string Answer { get; set; }

        // Date on which Answer was Posted
        public DateTime DatePosted { get; set; }

        // UserId as a Relational Field
        [ForeignKey("User")]
        public string UserId { get; set; }

        // QueryId as Relational Field
        [ForeignKey("Query")]
        public long QueryId { get; set; }

        // Navigation Properties

        // Declared virtual User Property for lazy loading Related Data
        public virtual ApplicationUser User { get; set; }

        // Declared virtual Query Property for lazy loading Related Data
        public virtual ForumQuery Query { get; set; }

        public ForumAnswer()
        {
            DatePosted = DateTime.Now;
        }
    }
}
