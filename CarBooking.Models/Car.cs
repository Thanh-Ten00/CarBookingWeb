using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CarBooking.Models
{
    
    public class Car
    {
        
        [Key]
        public int Id { get; set; }

        [DisplayName("ชื่อรถ")]

        public string? CarName { get; set; }
        [DisplayName("ประเภทรถ")]

        public string? CarType { get; set; }
        [DisplayName("จำนวนที่นั่ง")]
        public int? TotalSeats { get; set; } //จำนวนที่นั่งทั้งหมด
        [DisplayName("สถานะ")]
        public string? status { get; set; }  //สถานะ (พร้อมใช้/ที่เต็มแล้ว/อยู่ระหว่างใช้งาน/กำลังซ่อม)
        [DisplayName("ราคา/ระยะทาง(km)")]
        public float?  pricePerKm { get; set; }  //ราคาต่อระยะทาง()
        [DisplayName("จำนวนที่นั่งที่ถูกจองแล้ว")]
        public int BookedSeats { get; set; } = 0; //ที่นั่งที่ถูกจอง


        
        public int Displayorder { get; set; } = 2; //ลำดับการแสดงผล  seatremain = 1, available = 2, อื่นๆ = 3

        //หลังจากจอง
        [DisplayName("วันเริ่มเดินทาง")]
        public DateTime? Startdatetime { get; set; } //วันเริ่มเดินทาง
        [DisplayName("วันกลับ")]
        public DateTime? Returndatetime { get; set; } //วันกลับ
        [DisplayName("จุดขึ้นรถ")]
        public string? Origin { get; set; } //สถานที่เริ่มเดินทาง
        [DisplayName("จุดปลายทาง")]
        public string? Destination { get; set; } //สถานที่เจุดหมาย
        [DisplayName("ระยะทาง")]
        public string? Distance { get; set; } //ระยะทาง คำนวนจาก google api
        //public string? Duration { get; set; } 
        
    }
   
}
