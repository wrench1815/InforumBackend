using System.ComponentModel.DataAnnotations.Schema;

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

        // Category as a Navigation Property Justifying the Relation
        // Declared virtual for lazy loading Related Data
        public virtual Category Category { get; set; }

        public BlogPost()
        {
            DatePosted = DateTime.Now;
        }
    }
}
