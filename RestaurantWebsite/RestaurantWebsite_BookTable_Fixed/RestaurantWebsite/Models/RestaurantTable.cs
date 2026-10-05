using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.Models
{
    public class RestaurantTable
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string TableNumber { get; set; }

        [Range(1, 50)]
        public int Capacity { get; set; }

        [Required]
        [StringLength(50)]
        public string Zone { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        public RestaurantTable()
        {
            Reservations = new HashSet<Reservation>();
            Orders = new HashSet<Order>();
            Status = "Available";
            IsActive = true;
        }
    }
}
