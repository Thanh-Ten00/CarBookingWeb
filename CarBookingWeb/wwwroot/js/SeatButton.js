
var seatId = $(button).attr("id");;
var url = "/Employee/Booking/seatbooking?seatId=" + seatId;

function seatbooking(url, seatId) {
    Swal.fire({
        title: 'คุณแน่ใจหรือไม่?',
        text: 'เมื่อกดยืนยันระบบจะทำการจองที่นั่ง',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3A3B3C',
        confirmButtonText: 'ตกลง',
        cancelButtonText: 'ยกเลิก'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'PATCH',
                data: { seatId: seatId },
                success: function (data) {
                    if (data.success) {
                        Swal.fire({
                            title: 'จองที่นั่งสำเร็จ',
                            text: 'สถานะได้ถูกยืนยันแล้ว! ตรวจสอบได้ที่หน้ารายการจองรถ',
                            icon: 'success',
                            confirmButtonColor: '#3A3B3C',
                            confirmButtonText: 'รับทราบ'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                location.reload();
                            }
                        });
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function () {
                    toastr.error('เกิดข้อผิดพลาดในการส่งคำร้องขอ');
                }
            });
        }
    });
}




