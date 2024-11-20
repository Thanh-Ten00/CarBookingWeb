using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CarBooking.Models
{
    public class ApplicationUser : IdentityUser
    {


        [Required]
        public string? Name { get; set; }
        public string? Position { get; set; }
        public string? EmployeeNumber { get; set; }

    }
    
    
}
