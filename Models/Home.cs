using System.ComponentModel.DataAnnotations;

namespace InforumBackend.Models {
	public class Home {
		public long Id { get; set; }

		[Required]
		public string ImageLink { get; set; }
		
		[Required]
		public string Heading { get; set; }
		
		[Required]
		public string SubHeading { get; set; }
	}
}
