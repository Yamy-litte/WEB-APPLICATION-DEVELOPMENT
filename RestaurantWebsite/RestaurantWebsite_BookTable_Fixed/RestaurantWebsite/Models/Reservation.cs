using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int TableId { get; set; }

        [Required]
        public DateTime ReservationDate { get; set; }

        [Range(1, 50)]
        public int PartySize { get; set; }

        [StringLength(500)]
        public string SpecialRequest { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; }

        public virtual RestaurantTable Table { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public Reservation()
        {
            Orders = new HashSet<Order>();
            Status = "Pending";
            CreatedAt = DateTime.Now;
        }
    }
}
