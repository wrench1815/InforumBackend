namespace InforumBackend.Models {
	public class Comment {
		public long Id { get; set; }

		public string Content { get; set; }

		public DateTime DatePosted { get; set; }

		// TODO:
		//blog_post_id
		//user_id

		public Comment() {
			DatePosted = DateTime.Now;
		}
	}
}
