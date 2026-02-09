const searchInput = document.getElementById('searchInput');
searchInput.addEventListener('keyup', () => {
    const value = searchInput.value.toLowerCase();
    document.querySelectorAll('#discountTable tr').forEach(row => {
        row.style.display = row.innerText.toLowerCase().includes(value) ? '' : 'none';
    });
});


function onClickSaveDiscount() {
    var discount = new FormData();
    discount.append("Name", $('#txtNameDis').val());
    discount.append("PercentCal", $('#percentCal').val());
    discount.append("StartDate", $('#txtStartDate').val());
    discount.append("EndDate", $('#txtEndDate').val());
   

    $.ajax({
        url: addDiscountUrl,
        type: 'POST',
        data: discount,
        contentType: false,
        processData: false,
        success: function (res) {
            const modalItems = document.getElementById('discountModal');
            const Items = bootstrap.Modal.getInstance(modalItems);
            Items.hide();

            const Toast = Swal.mixin({
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.onmouseenter = Swal.stopTimer;
                    toast.onmouseleave = Swal.resumeTimer;
                }
            });
            Toast.fire({
                icon: "success",
                title: "Add Discount successfully"
            });

            setTimeout(() => {
                window.location.reload();
            }, 3000);
        }
    });
}

function onClickDeleteDiscount(Id) {
    Swal.fire({
        title: 'ยืนยันการลบ?',
        text: 'ข้อมูลนี้จะไม่สามารถกู้คืนได้',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'ลบ',
        cancelButtonText: 'ยกเลิก'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: deleteDiscountUrl,
                type: 'POST',
                data: { Id: id },
                success: function () {
                    const Toast = Swal.mixin({
                        toast: true,
                        position: "top-end",
                        showConfirmButton: false,
                        timer: 3000,
                        timerProgressBar: true,
                        didOpen: (toast) => {
                            toast.onmouseenter = Swal.stopTimer;
                            toast.onmouseleave = Swal.resumeTimer;
                        }
                    });
                    Toast.fire({
                        icon: "success",
                        title: "Deleted successfully"
                    });

                    setTimeout(() => {
                        window.location.reload();
                    }, 3000);
                },
                error: function () {
                    Swal.fire('Error', 'Can not Delete', 'error');
                }
            });
        }
    });
}

function onClickUpdateDiscount(Id) {

    var discount = new FormData();
    discount.append("Name", $('#txtUpdateNameDis').val());
    discount.append("PercentCal", $('#updatePercentCal').val());
    discount.append("StartDate", $('#txtUpdateStartDate').val());
    discount.append("EndDate", $('#txtUpdateEndDate').val());
    discount.append("Id", Id);

    $.ajax({
        url: updateDiscountUrl,
        type: 'POST',
        data: discount,
        contentType: false,
        processData: false,
        success: function (res) {
            const modalItems = document.getElementById('editdiscountModal_' + Id);
            const Items = bootstrap.Modal.getInstance(modalItems);
            Items.hide();

            const Toast = Swal.mixin({
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.onmouseenter = Swal.stopTimer;
                    toast.onmouseleave = Swal.resumeTimer;
                }
            });
            Toast.fire({
                icon: "success",
                title: "Update Discount successfully"
            });

            setTimeout(() => {
                window.location.reload();
            }, 3000);
        }
    });
}

function toggleDiscountStatus(id, isChecked) {
    $.ajax({
        url: updateStatusUrl,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            Id: id,
            StatusUse: isChecked ? 1 : 0
        }),
        success: function () {
            const Toast = Swal.mixin({
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.onmouseenter = Swal.stopTimer;
                    toast.onmouseleave = Swal.resumeTimer;
                }
            });
            Toast.fire({
                icon: "success",
                title: "Update Status successfully"
            });

            setTimeout(() => {
                window.location.reload();
                onClickShowTemplate(0);
            }, 3000);
        },
        error: function () {
            alert('Update failed');
        }
    });
}