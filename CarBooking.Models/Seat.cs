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
    public class Seat
    {
        [Key]
        public int Id { get; set; }
        public string? SeatCode { get; set; }
        public int CarId { get; set; }
        [ForeignKey("CarId")]
        [ValidateNever]
        public Car? Car { get; set; }

        public string? CarName { get; set; }
        public bool IsAvailable { get; set; } = true;

        

    }
}
