using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.Models
{
    public class Dish
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(0, 999999999)]
        public decimal Price { get; set; }

        [Range(1, 300)]
        public int PreparationTime { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<DishImage> DishImages { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<ComboItem> ComboItems { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        public Dish()
        {
            DishImages = new HashSet<DishImage>();
            OrderItems = new HashSet<OrderItem>();
            ComboItems = new HashSet<ComboItem>();
            Reviews = new HashSet<Review>();
            Status = "Available";
            CreatedAt = DateTime.Now;
        }
    }
}
