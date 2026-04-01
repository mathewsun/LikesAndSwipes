namespace LikesAndSwipes.Models
{
    public class UserProfilePhotosViewModel
    {
        public string UserName { get; set; } = string.Empty;

        public List<UserPhoto> Photos { get; set; } = new();
    }
}
