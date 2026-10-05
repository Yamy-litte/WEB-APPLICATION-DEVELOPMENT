using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.Models
{
    public class Combo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(0, 999999999)]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public virtual ICollection<ComboItem> ComboItems { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public Combo()
        {
            ComboItems = new HashSet<ComboItem>();
            OrderItems = new HashSet<OrderItem>();
            IsActive = true;
        }
    }
}
