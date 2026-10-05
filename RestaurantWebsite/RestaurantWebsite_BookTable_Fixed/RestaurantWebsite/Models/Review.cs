using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int? DishId { get; set; }

        public int? OrderId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsApproved { get; set; }

        [StringLength(1000)]
        public string RestaurantReply { get; set; }

        public DateTime? RestaurantReplyAt { get; set; }

        public virtual User User { get; set; }

        public virtual Dish Dish { get; set; }

        public virtual Order Order { get; set; }

        public Review()
        {
            CreatedAt = DateTime.Now;
            IsApproved = true;
        }
    }
}
