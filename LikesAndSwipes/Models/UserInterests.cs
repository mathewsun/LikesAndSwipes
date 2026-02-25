using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Densify;

namespace LikesAndSwipes.Models
{
    [PrimaryKey(nameof(UserId), nameof(InterestId))]
    public class UserInterests
    {
        public string UserId { get; set; }

        public long InterestId { get; set; }

        public DateTime Created { get; set; }
    }
}
