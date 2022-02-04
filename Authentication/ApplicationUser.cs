using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace InforumBackend.Authentication
{
    public enum Genders
    {
        Male, Female, Unspecified
    }
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }

        [PersonalData]
        public bool IsRestricted { get; set; }

        [PersonalData]
        public string ProfileImage { get; set; }


        [PersonalData]
        [Column(TypeName = "nvarchar(20)")]
        public Genders Gender { get; set; }

        [PersonalData]
        public DateTime DateJoined { get; set; }

        public ApplicationUser()
        {
            DateJoined = DateTime.Now;
        }

    }
}
