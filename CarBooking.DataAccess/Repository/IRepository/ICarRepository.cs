using CarBooking.DataAccess.Repository.IRepository;
using CarBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBooking.DataAccess.Repository.IRepository
{
    public interface ICarRepository : IRepository<Car>
    {
        IEnumerable<Car> GetMatchingCars(Car CarObj);

        
        void update(Car obj);
        
    }
}
