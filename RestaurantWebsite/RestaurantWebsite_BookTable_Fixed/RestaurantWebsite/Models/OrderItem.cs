using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int? DishId { get; set; }

        public int? ComboId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        [Range(0, 999999999)]
        public decimal UnitPrice { get; set; }

        [StringLength(500)]
        public string Note { get; set; }

        [Range(0, 999999999)]
        public decimal SubTotal { get; set; }

        public virtual Order Order { get; set; }

        public virtual Dish Dish { get; set; }

        public virtual Combo Combo { get; set; }
    }
}
