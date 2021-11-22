namespace InforumBackend.Models {
	public class Answer {
		public long Id { get; set; }

		public string? Content { get; set; }

		public DateTime DatePosted { get; set; }

		// TODO:
		//query_id
		//user_id

		public Answer() {
			DatePosted = DateTime.Now;
		}
	}
}
