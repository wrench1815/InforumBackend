using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InforumBackend.Authentication
{
    public class UpdateUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string ProfileImage { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public Genders Gender { get; set; }

        public string DOB { get; set; }

        public string Address { get; set; }

    }
}
