namespace LikesAndSwipes.Models
{
    public class UserProfilePhotosViewModel
    {
        public string UserId { get; set; } = string.Empty;

        public List<UserPhoto> Photos { get; set; } = new();

        public bool IsCurrentUserProfile { get; set; }
    }
}
