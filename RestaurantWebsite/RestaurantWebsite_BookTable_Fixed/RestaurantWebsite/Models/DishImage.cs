using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.Models
{
    public class DishImage
    {
        public int Id { get; set; }

        public int DishId { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; }

        public bool IsPrimary { get; set; }

        public virtual Dish Dish { get; set; }
    }
}
