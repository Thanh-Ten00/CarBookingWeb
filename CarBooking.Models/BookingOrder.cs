using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBooking.Models
{
    public class BookingOrder
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("SeatId")]
        [ValidateNever]
        public int SeatId { get; set; }
        public Seat? Seat { get; set; }

        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }


    }
}
