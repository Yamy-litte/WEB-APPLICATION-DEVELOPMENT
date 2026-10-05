using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.Models
{
    public class OrderPromotion
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int PromotionId { get; set; }

        [Range(0, 999999999)]
        public decimal DiscountAmount { get; set; }

        public virtual Order Order { get; set; }

        public virtual Promotion Promotion { get; set; }
    }
}
