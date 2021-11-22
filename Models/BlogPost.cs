using System.ComponentModel.DataAnnotations.Schema;

namespace InforumBackend.Models {
	public class BlogPost {
		public long Id { get; set; }

		public string? Title { get; set; }

		public string? Description { get; set; }

		public string? Excerpt { get; set; }

		public string? Slug { get; set; }

		public DateTime DatePosted { get; set; }

		// TODO:
		//author_id
		//category_id

		public BlogPost() {
			DatePosted = DateTime.Now;
		}
	}
}
