namespace LikesAndSwipes.Models
{
    public class UserProfilePhotosViewModel
    {
        public string UserName { get; set; } = string.Empty;

        public DateTime? BirthDay { get; set; }

        public List<UserPhoto> Photos { get; set; } = new();
    }
}
