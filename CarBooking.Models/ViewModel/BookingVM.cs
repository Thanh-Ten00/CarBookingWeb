using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBooking.Models.ViewModel
{

	public class BookingVM
	{
        

        public IEnumerable<Seat> Seat { get; set; }
        public IEnumerable<Car> Car { get; set; }

        public IEnumerable<BookingOrder> bookingOrder { get;set; }

		
    }
}
