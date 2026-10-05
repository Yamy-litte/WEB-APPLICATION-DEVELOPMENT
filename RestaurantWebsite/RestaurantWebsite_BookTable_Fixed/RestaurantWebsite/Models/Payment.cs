using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        [Required]
        [StringLength(30)]
        public string PaymentMethod { get; set; }

        [Range(0, 999999999)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; }

        public DateTime PaymentDate { get; set; }

        [StringLength(100)]
        public string TransactionCode { get; set; }

        public virtual Order Order { get; set; }
    }
}
