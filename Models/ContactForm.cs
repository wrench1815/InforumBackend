using System.ComponentModel.DataAnnotations;

namespace InforumBackend.Models {
	public class ContactForm {
		public long Id { get; set; }

		[Required]
		public string FullName { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		public string Message { get; set; }
		public DateTime CreatedOn { get; set; }

		public ContactForm() {
			CreatedOn = DateTime.Now;
		}
	}
}
