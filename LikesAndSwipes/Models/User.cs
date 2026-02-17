using Microsoft.AspNetCore.Identity;

namespace LikesAndSwipes.Models
{
    public class User : IdentityUser
    {
        public DateTime Created { get; set; }
    }
}
