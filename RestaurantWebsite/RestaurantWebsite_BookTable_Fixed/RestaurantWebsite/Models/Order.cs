using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int? ReservationId { get; set; }

        public int? TableId { get; set; }

        [Required]
        [StringLength(30)]
        public string OrderType { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; }

        [StringLength(500)]
        public string DeliveryAddress { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [Range(0, 999999999)]
        public decimal SubTotal { get; set; }

        [Range(0, 999999999)]
        public decimal DiscountAmount { get; set; }

        [Range(0, 999999999)]
        public decimal DeliveryFee { get; set; }

        [Range(0, 999999999)]
        public decimal TotalAmount { get; set; }

        [StringLength(500)]
        public string Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; }

        public virtual Reservation Reservation { get; set; }

        public virtual RestaurantTable Table { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }

        public virtual ICollection<OrderPromotion> OrderPromotions { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
            Payments = new HashSet<Payment>();
            OrderPromotions = new HashSet<OrderPromotion>();
            Reviews = new HashSet<Review>();

            Status = "Pending";
            CreatedAt = DateTime.Now;
        }
    }
}
