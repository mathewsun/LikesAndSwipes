using System.ComponentModel.DataAnnotations;

namespace LikesAndSwipes.Models
{
    public class UserPhoto
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(512)]
        public string ObjectName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string ContentType { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public DateTime Created { get; set; }

        public User? User { get; set; }
    }
}
