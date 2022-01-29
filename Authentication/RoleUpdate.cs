using System.ComponentModel.DataAnnotations;

namespace InforumBackend.Models
{
    public class RoleUpdate
    {
        [Required(ErrorMessage = "RoleId is required")]
        public string RoleId { get; set; }

        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
    }
}
