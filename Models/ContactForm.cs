namespace InforumBackend.Models {
	public class ContactForm {
		public long Id { get; set; }

		public string FullName { get; set; }

		public string Email { get; set; }

		public string Message { get; set; }

		public DateTime CreatedOn { get; set; }

		public ContactForm() {
			CreatedOn = DateTime.Now;
		}
	}
}
