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

        /// <summary>
        ///     Romantic preference for men
        /// </summary>
        public bool RomanticMen { get; set; }

        /// <summary>
        ///     Romantic preference for women
        /// </summary>
        public bool RomanticWomen { get; set; }

        /// <summary>
        ///     Friendship preference for men
        /// </summary>
        public bool FriendshipMen { get; set; }

        /// <summary>
        ///     Friendship preference for women
        /// </summary>
        public bool FriendshipWomen { get; set; }

        public int Age { get; set; }

        public DateTime BirthDay { get; set; }

        public string Address { get; set; }

        public ICollection<UserPhoto> Photos { get; set; } = new List<UserPhoto>();
    }
}
