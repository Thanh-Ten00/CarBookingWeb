using CarBooking.DataAccess.data;
using CarBooking.DataAccess.Repository;
using CarBooking.DataAccess.Repository.IRepository;
using CarBooking.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBooking.DataAccess.Repository
{
    public class CarRepository : Repository<Car>, ICarRepository
    {
        private ApplicationDBcontext _db;
        public CarRepository(ApplicationDBcontext db): base(db)
        {
            _db = db;
        }

        public IEnumerable<Car> GetMatchingCars(Car CarObj)
        {
            IQueryable<Car> query = _db.Cars;
            


            if (!string.IsNullOrEmpty(CarObj.CarName))
            {
                query = query.Where(c => c.CarName.ToLower().Contains(CarObj.CarName.ToLower()));
            }

            if (CarObj.TotalSeats != null)
            {
                query = query.Where(c => c.TotalSeats == CarObj.TotalSeats);
            }

            if (CarObj.CarType != null)
            {
                query = query.Where(c => c.CarType.ToLower().Contains(CarObj.CarType.ToLower()));
            }

            


            


            return query.ToList();
        }

        


        public void update(Car obj)
        {
            _db.Cars.Update(obj);
        }
    }
}
