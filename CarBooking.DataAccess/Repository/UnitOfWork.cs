using CarBooking.DataAccess.data;
using CarBooking.DataAccess.Repository;
using CarBooking.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDBcontext _db;
        public ICarRepository CarRepo { get; private set; }

        public ISeatRepository SeatRepo { get; private set; }

        public IBookingRepository BookingRepo { get; private set; }

        public IApplicationUserRepository UserRepo { get; private set; }

        //get; private set;: การกำหนดการเข้าถึงค่าของคุณสมบัติ productRepo โดยมีการกำหนด
        //get ให้สามารถอ่านค่าได้
        //และ private set ที่บังคับให้สามารถกำหนดค่าได้เฉพาะภายในคลาสเท่านั้น นั่นหมายความว่าค่าของ productRepo สามารถเข้าถึงได้จากภายนอก
        //แต่การกำหนดค่าต้องทำภายในคลาสเท่านั้น

        public UnitOfWork(ApplicationDBcontext db)
        { 
            _db = db;
            CarRepo = new CarRepository(_db);
            SeatRepo = new SeatRepository(_db);
            UserRepo = new ApplicationUserRepository(_db);
            BookingRepo = new BookingRepository(_db);

        }

        public void EnableIdentityInsert(string tableName)
        {
            string sql = $"SET IDENTITY_INSERT {tableName} ON";
            _db.Database.ExecuteSqlRaw(sql);
        }

        public void DisableIdentityInsert(string tableName)
        {
            string sql = $"SET IDENTITY_INSERT {tableName} OFF";
            _db.Database.ExecuteSqlRaw(sql);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
