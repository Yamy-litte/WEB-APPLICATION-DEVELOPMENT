using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(250)]
        public string ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Dish> Dishes { get; set; }

        public Category()
        {
            Dishes = new HashSet<Dish>();
            IsActive = true;
        }
    }
}
