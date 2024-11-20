using Bulky.DataAccess.Repository;
using CarBooking.DataAccess.data;
using CarBooking.DataAccess.Repository;
using CarBooking.DataAccess.Repository.IRepository;
using CarBooking.Models;
using CarBooking.Models.ViewModel;
using CarBooking.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System;
using System.Data;
using System.Runtime.ConstrainedExecution;
using System.Xml.XPath;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
//using CarBooking.DataAccess.Migrations;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CarBookingWeb.Areas.Employee.Controllers
{
    [Area("Employee")]
    //[Authorize(Roles = SD.Role_Employee)]
    [Authorize]

    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly HttpClient _httpClient;

        [BindProperty]
        public BookingVM bookingVM { get; set; }



        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _httpClient = new HttpClient();
        }

        public IActionResult Index() //ตาราง มีปุ่มเพิ่มรายการจอง
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            IEnumerable<BookingOrder> bookingOrder = _unitOfWork.BookingRepo.GetAll(filter: u => u.ApplicationUserId == userid, includeProperties: "Seat").ToList();

            IEnumerable<Seat> seatList = bookingOrder.Select(order => order.Seat).ToList();

            IEnumerable<Car> carList = seatList.Select(seat =>
            {
                var car = _unitOfWork.CarRepo.Get(c => c.Id == seat.CarId);
                return car;
            }).ToList();

            bookingVM = new BookingVM()
            {
                bookingOrder = bookingOrder,
                Seat = seatList,
                Car = carList
            };
            return View(bookingVM);
        }


        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search(Car CarObj)
        {

            IEnumerable<Car> cars = _unitOfWork.CarRepo.GetMatchingCars(CarObj);

            //ลำดับการแสดงผล ไม่ต้องอัพเดท database  seatremain = 1, available = 2, อื่นๆ = 3 
            foreach (Car car in cars)
            {
                if (car.status == SD.StatusSeatsRemain)
                {
                    car.Displayorder = 2;
                }
                else if (car.status == SD.StatusAvailable)
                {
                    car.Displayorder = 3;
                }
                else
                {
                    car.Displayorder = 4;
                }
            }

            var displayseat = _unitOfWork.SeatRepo.GetSeatToDisplay(cars);

            ViewData["Cars"] = cars;
            ViewData["Seat"] = displayseat;

            return View();

        }



        //#region API CALLS

        //[HttpPatch]
        public IActionResult seatbooking(int seatId) //เช็ค car.startdate,returndate,origin,destination ถ้าค่าพวกนี้เป็น null ให้ redirect to startorder
        {
  
            Seat seat = _unitOfWork.SeatRepo.Get(u => u.Id == seatId);
            Car car = _unitOfWork.CarRepo.Get(c => c.Id == seat.CarId);


            //เช็คว่ารถคันที่เลือก มีการกำหนดวันไปกลับรึยัง 
            if (car.Startdatetime != null)
            {
                BookingOrder order = new()
                {
                    Seat = seat,
                    SeatId = seatId
                };

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                order.ApplicationUserId = userid;

                BookingOrder orderFromDb = _unitOfWork.BookingRepo.Get(u => u.ApplicationUserId == userid && u.SeatId == order.SeatId);

                if (orderFromDb != null)
                {
                    _unitOfWork.BookingRepo.Remove(orderFromDb);

                }
                else
                {
                    _unitOfWork.BookingRepo.Add(order);
                }


                seat.IsAvailable = !seat.IsAvailable;
                if (seat.IsAvailable == true)
                {

                    car.BookedSeats -= 1;
                }
                else if (seat.IsAvailable == false)
                {
                    car.BookedSeats += 1;

                }


                //ทุกครั้งที่เพิ่มการจอง จะเช็คว่าที่นั่งเต็มรึยัง ถ้าเต็มแล้ว ตั้งสถานะ statusseatfully ถ้ายังไม่เต็ม SeatsRemain ถ้าก่อนจองสถานะเป็น available ให้เพิ่มเวลาไปกลับไปที่รถ
                if (car.BookedSeats == 0)
                {
                    // ถ้ายังไม่มีการจองที่นั่ง
                    car.status = SD.StatusAvailable;
                }
                else if (car.BookedSeats == car.TotalSeats)
                {
                    // ถ้าที่นั่งเต็มแล้ว
                    car.status = SD.StatusSeatsFully;

                }
                else
                {
                    // ถ้าที่นั่งยังไม่เต็ม
                    car.status = SD.StatusSeatsRemain;
                }



                //ทุกครั้งที่ยกเลิกการจอง จะเช็คว่าจำนวนที่จอง เท่ากับ 0 มั้ย ถ้าใช่ตั้งสถานะ available ถ้าจำนวนที่จองมากกว่า 0 แต่ไม่เท่าจำนวนสูงสุด ตั้งสถานะ SeatsRemain
                //ยกเลิกการจองทำได้โดยคนที่กดจอง โดยยกเลิกในหน้ารายการจอง(Index) 
                //ข้อมูลการจองมีวันไป-กลับ ระยะทาง ค่าใช้จ่ายต่อระยะทาง

                _unitOfWork.CarRepo.update(car);
                _unitOfWork.SeatRepo.update(seat);
                _unitOfWork.Save();

                return Json(new { success = true, seatId });

            }
            else
            {
                return RedirectToAction(nameof(Seatbookingstartorder), new { seatId = seat.Id});
            }
            
        }


        //เข้ามาผ่าน seatbooking ใช้ redirecttoaction ส่งค่า carid รถคันที่เลือก
        //หน้า form กรณีที่รถยังไม่เคยมีคนจอง จะให้คนที่จองคนแรก 
        public IActionResult Seatbookingstartorder(int seatId)
        {
            var seat = _unitOfWork.SeatRepo.Get(u => u.Id == seatId);
            var CarFromDB = _unitOfWork.CarRepo.Get(c => c.Id == seat.CarId);

            TempData["SId"] = seatId;
            

            return View(CarFromDB);
        }


        [HttpPost]
        public IActionResult Seatbookingstartorder(Car carObj)
        {
            int seatId = (int)TempData["SId"];
            var seat = _unitOfWork.SeatRepo.Get(u => u.Id == seatId);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            BookingOrder order = new()
            {
                SeatId = seat.Id,
                ApplicationUserId = userid
            };

            
            carObj.BookedSeats += 1;

            if (carObj.BookedSeats == 0)
            {
                // ถ้ายังไม่มีการจองที่นั่ง
                carObj.status = SD.StatusAvailable;
            }
            else if (carObj.BookedSeats == carObj.TotalSeats)
            {
                // ถ้าที่นั่งเต็มแล้ว
                carObj.status = SD.StatusSeatsFully;

            }
            else
            {
                // ถ้าที่นั่งยังไม่เต็ม
                carObj.status = SD.StatusSeatsRemain;
            }



            string apiKey = "YOUR_API_KEY";

            string apiUrl = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={carObj.Origin}&destinations={carObj.Destination}&key={apiKey}";



            HttpResponseMessage response;

            using (var httpClient = new HttpClient())
            {
                response = httpClient.GetAsync(apiUrl).Result;
            }

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                carObj.Distance = result.rows[0].elements[0].distance.text;
            }

            seat.IsAvailable = !seat.IsAvailable;

            _unitOfWork.SeatRepo.update(seat);
            _unitOfWork.CarRepo.update(carObj);
            _unitOfWork.BookingRepo.Add(order);
            _unitOfWork.Save();

            TempData["success"] = $"สร้างรายการจอง {carObj.CarName} เรียบร้อย!!";


            return RedirectToAction(nameof(Search));
        }


        public IActionResult CancelOrder(int? id)
        {
            Seat SeatTobeCancel = _unitOfWork.SeatRepo.Get(u => u.Id == id);
            Car CarOfThisSeat = _unitOfWork.CarRepo.Get(u => u.Id == SeatTobeCancel.CarId);
            BookingOrder OrderTobeDeleted = _unitOfWork.BookingRepo.Get(u => u.SeatId == SeatTobeCancel.Id);


            if (SeatTobeCancel == null)
            {
                return NotFound();
            }

            SeatTobeCancel.IsAvailable = !SeatTobeCancel.IsAvailable;
            CarOfThisSeat.BookedSeats--;

            if (CarOfThisSeat.BookedSeats == 0)
            {
                // ถ้ายังไม่มีการจองที่นั่ง
                CarOfThisSeat.status = SD.StatusAvailable;
                CarOfThisSeat.Startdatetime = null;
                CarOfThisSeat.Returndatetime = null;
                CarOfThisSeat.Origin = null;
                CarOfThisSeat.Destination = null;
                CarOfThisSeat.Distance = null;
            }
            else if (CarOfThisSeat.BookedSeats == CarOfThisSeat.TotalSeats)
            {
                // ถ้าที่นั่งเต็มแล้ว
                CarOfThisSeat.status = SD.StatusSeatsFully;

            }
            else
            {
                // ถ้าที่นั่งยังไม่เต็ม
                CarOfThisSeat.status = SD.StatusSeatsRemain;
            }

            



            _unitOfWork.BookingRepo.Remove(OrderTobeDeleted);
            _unitOfWork.SeatRepo.update(SeatTobeCancel);
            _unitOfWork.CarRepo.update(CarOfThisSeat);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }



        //#endregion

    }
}
