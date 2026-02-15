using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace LikesAndSwipes.Models
{
    public class LocationEntity
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public Point Location { get; set; } // PostGIS Point

        public DateTime Created { get; set; }
    }
}
