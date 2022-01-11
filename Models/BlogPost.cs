using System.ComponentModel.DataAnnotations.Schema;
using InforumBackend.Authentication;

namespace InforumBackend.Models
{
    public class BlogPost
    {
        // Primary Key
        public long Id { get; set; }

        // Title of the Blog Post
        public string Title { get; set; }

        // Content of the Blog Post
        public string Description { get; set; }

        // Excerpt of the Blog Post
        // TODO: set text limit
        public string Excerpt { get; set; }

        // Slug of the Blog Post, Post to be accessed via Slug rather than pk/Id
        public string Slug { get; set; }

        // Date the Blog Post was Added
        public DateTime DatePosted { get; set; }

        // TODO:
        //author_id

        // CategoryId as a Relational Field
        [ForeignKey("Category")]
        public long CategoryId { get; set; }

        [ForeignKey("Author")]
        public string AuthorId { get; set; }

        // Navigation Properties

        // Declared virtual Category Property for lazy loading Related Data
        public virtual Category Category { get; set; }

        // Declared virtual Author Property for lazy loading Related Data
        public virtual ApplicationUser Author { get; set; }
        public BlogPost()
        {
            DatePosted = DateTime.Now;
        }
    }
}
