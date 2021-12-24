using System.ComponentModel.DataAnnotations;

namespace InforumBackend.Authentication
{
    public class UpdatePassword
    {

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

    }
}
