using Contracts.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    [Index(nameof(CartId), nameof(CourseId), IsUnique = true)]
    public class CartItem : EntityBase<int>
    {
        [ForeignKey("Cart")]
        public int CartId { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }


        // Navigation properties
        public virtual Cart Cart { get; set; }
        public virtual Course Course { get; set; }
    }
}