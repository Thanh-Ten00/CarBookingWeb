using CarBooking.DataAccess.data;
using CarBooking.DataAccess.Repository;
using CarBooking.DataAccess.Repository.IRepository;
using CarBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBooking.DataAccess.Repository
{
    public class BookingRepository : Repository<BookingOrder>, IBookingRepository
    {
        private ApplicationDBcontext _db;
        public BookingRepository(ApplicationDBcontext db): base(db)
        {
            _db = db;
        }

        

        public void update(BookingOrder obj)
        {
            _db.BookingOrders.Update(obj);
        }
    }
}
