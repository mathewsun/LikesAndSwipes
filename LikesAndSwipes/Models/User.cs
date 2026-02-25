using Microsoft.AspNetCore.Identity;

namespace LikesAndSwipes.Models
{
    public class User : IdentityUser
    {
        public DateTime Created { get; set; }

        public string FirstName { get; set; }

        public bool Sex { get; set; } //0 - man, 1 - woman

        public bool IsHomosexual { get; set; }

        public bool IsBisexual { get; set; }

        public int Age { get; set; }

        public DateTime BirthDay { get; set; }
    }
}
