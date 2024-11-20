using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBooking.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICarRepository CarRepo { get; }

        ISeatRepository SeatRepo { get; }

        IBookingRepository BookingRepo { get; }

        void DisableIdentityInsert(string tableName);

        void EnableIdentityInsert(string tableName);

        void Save();
    }
}
