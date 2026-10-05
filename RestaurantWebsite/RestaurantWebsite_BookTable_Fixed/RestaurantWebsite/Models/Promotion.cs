using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(30)]
        public string DiscountType { get; set; }

        [Range(0, 999999999)]
        public decimal DiscountValue { get; set; }

        [Range(0, 999999999)]
        public decimal MinimumOrderAmount { get; set; }

        public decimal? MaximumDiscount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? UsageLimit { get; set; }

        public int UsedCount { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<OrderPromotion> OrderPromotions { get; set; }

        public Promotion()
        {
            OrderPromotions = new HashSet<OrderPromotion>();
            IsActive = true;
        }
    }
}
