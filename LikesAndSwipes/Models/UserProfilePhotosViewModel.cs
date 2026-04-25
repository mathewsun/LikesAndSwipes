namespace LikesAndSwipes.Models
{
    public class UserProfilePhotosViewModel
    {
        public string UserName { get; set; } = string.Empty;

        public DateTime? BirthDay { get; set; }
        public bool RomanticMen { get; set; }
        public bool RomanticWomen { get; set; }
        public bool FriendshipMen { get; set; }
        public bool FriendshipWomen { get; set; }

        public List<UserPhoto> Photos { get; set; } = new();
    }
}
