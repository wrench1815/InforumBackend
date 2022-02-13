using System.ComponentModel.DataAnnotations.Schema;
using InforumBackend.Authentication;

namespace InforumBackend.Models
{
    public class ForumQuery
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Slug { get; set; }

        public DateTime DatePosted { get; set; }

        public long Vote { get; set; }

        // CategoryId as a Relational Field
        [ForeignKey("Category")]
        public long CategoryId { get; set; }

        // AuthorId as a Relational Field
        [ForeignKey("Author")]
        public string AuthorId { get; set; }

        // Navigation Properties

        // Declared virtual Category Property for lazy loading Related Data
        public virtual Category Category { get; set; }

        // Declared virtual Author Property for lazy loading Related Data
        public virtual ApplicationUser Author { get; set; }
        public ForumQuery()
        {
            DatePosted = DateTime.Now;
        }
    }
}
