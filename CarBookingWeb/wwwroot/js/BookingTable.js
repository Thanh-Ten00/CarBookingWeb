var dataTable;



$(document).ready(function () {


    loadDataTable();

})



function loadDataTable() {

    dataTable = $("#tblData").DataTable({

        "ajax": {

            "url": "/Employee/booking/getall"

        },

        "columns": [

            { "data": "carName", "width": "10%" },

            { "data": "carType", "width": "10%" },

            { "data": "totalSeats", "width": "10%" },

            { "data": "status", "width": "15%" },

            { "data": "pricePerKm", "width": "15%" },

            {
                "data": "id",
                "render": function (data, type, row) {
                    var buttonLabel = row.status === 'Fixed' ? 'ซ่อมเสร็จแล้ว' : 'แจ้งซ่อม';
                    var buttonClass = row.status === 'Fixed' ? 'btn-danger' : 'btn-warning';
                    return `<div class=" d-flex btn-group" role="group">
                            <a href="/Admin/Car/Upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i>แก้ไข</a>
                             <a onClick=ToggleFixed('/Admin/Car/ToggleFixed/${data}') class="btn ${buttonClass} mx-2"><i class="bi bi-pencil-square"></i>${buttonLabel}</a>
                            
                            <a onClick=Delete('/Admin/Car/Delete/${data}') class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i>ลบข้อมูล</a>
					    </div>
                        `
                }
                , "width": "40%"
            }
        ]
    });
}

//sweet Alert2 (2) 
function Delete(url) {
    Swal.fire({
        title: 'คุณแน่ใจหรือไม่?',
        text: "หลังจากลบข้อมูลคุณจะเรียกกลับคืนมาไม่ได้!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3A3B3C',
        confirmButtonText: 'ใช่ , ลบมันเลย!',
        cancelButtonText: 'ยกเลิกการลบข้อมูล'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        Swal.fire({
                            title: 'ลบข้อมูลสำเร็จ',
                            text: "ข้อมูลได้ถูกลบแล้ว!",
                            icon: 'success',
                            confirmButtonColor: '#3A3B3C',
                            confirmButtonText: 'รับทราบ'

                        })
                        //toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}