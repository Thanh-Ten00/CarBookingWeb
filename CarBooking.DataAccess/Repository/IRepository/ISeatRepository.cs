using CarBooking.DataAccess.Repository.IRepository;
using CarBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBooking.DataAccess.Repository.IRepository
{
    public interface ISeatRepository : IRepository<Seat>
    {
       
        void update(Seat obj);

        IEnumerable<Seat> GetSeat(int car);

        IEnumerable<Seat> GetSeatToDisplay(IEnumerable<Car> cars);


    }
}
