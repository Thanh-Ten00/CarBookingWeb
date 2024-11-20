using Bulky.DataAccess.Repository;
using CarBooking.DataAccess.data;
using CarBooking.DataAccess.Repository;
using CarBooking.DataAccess.Repository.IRepository;
using CarBooking.Models;

using CarBooking.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Xml.XPath;

namespace CarBookingWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CarController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly ISeatService _seatService;


        public CarController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }


        public IActionResult Index()
        {
            List<Car> objCarList = _unitOfWork.CarRepo.GetAll().ToList();

            return View(objCarList);
        }

        public IActionResult Upsert(int? id) //update+insert
        {
            if (id == null || id == 0)
            {
                //create
                return View(new Car());
            }
            else
            {
                //update
                Car CarObj = _unitOfWork.CarRepo.Get(u => u.Id == id);
                
                return View(CarObj);
            }

        }

        [HttpPost]
        public IActionResult Upsert(Car CarObj)
        {
            if (ModelState.IsValid)
            {
                if (CarObj.Id == 0)
                {
                    CarObj.status = SD.StatusAvailable;
                    _unitOfWork.CarRepo.Add(CarObj);

                    _unitOfWork.Save();


                    if (CarObj.TotalSeats > 0)
                    {
                        for (int i = 0; i < CarObj.TotalSeats; i++)
                        {
                            Seat seat = new Seat
                            {
                                CarId = CarObj.Id,
                                SeatCode = CarObj.CarName + "_S00" + (i + 1).ToString(),
                                CarName = CarObj.CarName
                            };

                            _unitOfWork.SeatRepo.Add(seat);
                        }
                    }
                    _unitOfWork.Save();

                    
                }
                else
                {
                    List<Seat> SeatToBeDeleted = _unitOfWork.SeatRepo.GetAll(u => u.CarId == CarObj.Id).ToList();
                    _unitOfWork.SeatRepo.RemoveRange(SeatToBeDeleted);
                    for (int i = 0; i < CarObj.TotalSeats; i++)
                    {
                        Seat seat = new Seat
                        {
                            CarId = CarObj.Id,
                            SeatCode = CarObj.CarName + "_S00" + (i + 1).ToString(),
                            CarName = CarObj.CarName
                        };

                        _unitOfWork.SeatRepo.Add(seat);
                    }

                    CarObj.status = SD.StatusAvailable;
                    _unitOfWork.CarRepo.update(CarObj);


                    _unitOfWork.Save();
                }

                return RedirectToAction("Index");

            }
            else
            {
                
                return View(CarObj);
            }


        }


        #region API CALLS
        [HttpGet]
         public IActionResult GetAll() {
            List<Car> objCarList = _unitOfWork.CarRepo.GetAll().ToList();
            return Json(new { data = objCarList });
        }

        [HttpPatch]
        public IActionResult ToggleFixed(int? id)
        {
            Car CarToBeFixed = _unitOfWork.CarRepo.Get(u => u.Id == id);
            if (CarToBeFixed == null)
            {
                return Json(new { success = false, message = "ไม่สามารถเลือกรายการนี้ได้" });
            }

            

            if (CarToBeFixed.status != SD.StatusFixed)
            {
                CarToBeFixed.BookedSeats = 0;
                CarToBeFixed.status = SD.StatusFixed;
                CarToBeFixed.Startdatetime = null;
                CarToBeFixed.Returndatetime = null;
                CarToBeFixed.Origin = null;
                CarToBeFixed.Destination = null;
                CarToBeFixed.Distance = null;

                IEnumerable<Seat> seats = _unitOfWork.SeatRepo.GetSeat(CarToBeFixed.Id);
                 
                //รีเซ็ต Bookingorder รายการจองของรถคันนี้ทั้งหมด 
                foreach (Seat seat in seats)
                {
                    seat.IsAvailable = true;
                    
                    BookingOrder orderFromDb = _unitOfWork.BookingRepo.Get(u => u.SeatId == seat.Id);

                    if (orderFromDb != null && orderFromDb.SeatId == seat.Id)
                    {
                        _unitOfWork.BookingRepo.Remove(orderFromDb);
                        _unitOfWork.SeatRepo.update(seat);
                    }

                }

            }
            else
            {
                CarToBeFixed.status = SD.StatusAvailable;
               
            }   


            _unitOfWork.CarRepo.update(CarToBeFixed);
            _unitOfWork.Save();

            return Json(new { success = true, message = "รถคันนี้กำลังส่งซ่อม" });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CarToBeDeleted = _unitOfWork.CarRepo.Get(u => u.Id == id);
            List<Seat> SeatToBeDeleted = _unitOfWork.SeatRepo.GetAll(u => u.CarId == id).ToList();
            if (CarToBeDeleted == null)
            {
                return Json(new { success = false, message = "ไม่สามารถลบรายการนี้ได้" });
            }


            _unitOfWork.SeatRepo.RemoveRange(SeatToBeDeleted);
            _unitOfWork.CarRepo.Remove(CarToBeDeleted);
            _unitOfWork.Save();
           
            return Json(new { success = true, message = "ลบรถคันนี้ออกจากระบบแล้ว" });
        }
        #endregion

    }
}
