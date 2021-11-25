namespace InforumBackend.Models {
	public class Category {
		public long Id { get; set; }
		public string? Name { get; set; }

		public ICollection<BlogPost>? BlogPost { get; set; }

	}
}
