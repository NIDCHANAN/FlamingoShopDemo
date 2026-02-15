let selectedFiles = [];
let deletedImages = [];

const input = document.getElementById("imageUpload");
const preview = document.getElementById("imagePreview");

input.addEventListener("change", () => {
    Array.from(input.files).forEach(file => {
        if (!file.type.startsWith("image/")) return;

        selectedFiles.push(file);
        const index = selectedFiles.length - 1;

        const wrapper = document.createElement("div");
        wrapper.className = "position-relative";

        const img = document.createElement("img");
        img.src = URL.createObjectURL(file);
        img.style.width = "120px";
        img.style.height = "120px";
        img.style.objectFit = "cover";
        img.className = "rounded border";

        const btn = document.createElement("button");
        btn.type = "button";
        btn.innerHTML = "✕";
        btn.className = "btn btn-danger btn-sm position-absolute top-0 end-0";
        btn.style.transform = "translate(40%, -40%)";

        btn.onclick = () => {
            selectedFiles.splice(index, 1);
            wrapper.remove();
        };

        wrapper.appendChild(img);
        wrapper.appendChild(btn);
        preview.appendChild(wrapper);
    });

    // reset input เพื่อเลือกไฟล์เดิมซ้ำได้
    input.value = "";
});

function removeOldImage(imageId) {
    deletedImages.push(imageId);

    $.ajax({
        url: deletedImageUrl,
        type: 'POST',
        data: {
            Id: imageId
        },
        success: function () {
            document.querySelector(`[onclick="removeOldImage(${imageId})"]`)
                .closest("div")
                .remove();
        },
        error: function () {
            alert('Update failed');
        }
    });


}

function onClickSaveUser(e) {
    e.preventDefault();

    var user = new FormData();
    user.append("Name", $('#txtName').val());
    user.append("Email", $('#txtEmail').val());
    user.append("Password", $('#passwordInput').val());
    user.append("Telephone", $('#txtTelephone').val());
    user.append("Address", $('#txtAddress').val());
    user.append("Status", $('#selectStatus').val());
    user.append("Role", $('#selectRole').val());
    selectedFiles.forEach(file => {
        user.append("Images", file);
    });

    $.ajax({
        url: addUserUrl,
        type: 'POST',
        data: user,
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.result === "duplicate") {
                Swal.fire({
                    icon: 'error',
                    title: 'เบอร์โทรซ้ำ',
                    confirmButtonText: 'ตกลง'
                }).then(() => { $('#txtTelephone').focus(); });
                return false; 
            }

            if (res.result === "success") {
                const modalItems = document.getElementById('userModal');
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
                    title: "Add User successfully"
                });

                setTimeout(() => {
                    window.location.reload();
                }, 3000);
            }
        }
       
    });
}

function onClickDeleteUser(id) {
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
                url: deleteUserUrl,
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

function onClickUpdateUser(id) {

    var user = new FormData();
    var user = new FormData();
    user.append("Name", $('#txtEditName_' + id ).val());
    user.append("Email", $('#txtEditEmail_' + id).val());
    user.append("Password", $('#passwordEditInput_' + id).val());
    user.append("Telephone", $('#txtEditTelephone_' + id).val());
    user.append("Address", $('#txtEditAddress_' + id).val());
    user.append("Status", $('#selectEditStatus_' + id).val());
    user.append("Role", $('#selectEditRole_' + id).val());
    user.append("Id", id);
    selectedFiles.forEach(file => {
        user.append("Images", file);
    });

    $.ajax({
        url: updateUserUrl,
        type: 'POST',
        data: user,
        contentType: false,
        processData: false,
        success: function (res) {
            const modalItems = document.getElementById('editUserModal_' + id);
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
                title: "Update User successfully"
            });

            setTimeout(() => {
                window.location.reload();
            }, 3000);
        }
    });
}

function previewImages(Id) {
    const input = document.getElementById(`imageUpload_${Id}`);
    const preview = document.getElementById(`imagePreview_${Id}`);

    if (!input.files || input.files.length === 0) return;

    Array.from(input.files).forEach(file => {
        if (!file.type.startsWith("image/")) return;

        selectedFiles.push(file);

        const img = document.createElement("img");
        img.src = URL.createObjectURL(file);
        img.style.width = "120px";
        img.style.height = "120px";
        img.style.objectFit = "cover";
        img.className = "rounded border";

        preview.appendChild(img);
    });

    input.value = "";
}

function removeOldImage(imageId) {
    deletedImages.push(imageId);

    $.ajax({
        url: deletedImageUrl,
        type: 'POST',
        data: {
            Id: imageId
        },
        success: function () {
            document.querySelector(`[onclick="removeOldImage(${imageId})"]`)
                .closest("div")
                .remove();
        },
        error: function () {
            alert('Update failed');
        }
    });


}

function onClickBlackList(id) {
    Swal.fire({
        title: 'ยืนยันการปืดกั้นผู้ใช้งาน?',
        text: 'ผู้ใช้งานจะไม่สามารถเข้าสู่ระบบได้',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'ยืนยัน',
        cancelButtonText: 'ยกเลิก'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: lockUserUrl,
                type: 'POST',
                data: { Id: id, type : 1 },
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
                        title: "Locked successfully"
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



function onClickUnBlackList(id) {
    Swal.fire({
        title: 'ยืนยันการปืดกั้นผู้ใช้งาน?',
        text: 'ผู้ใช้งานจะไม่สามารถเข้าสู่ระบบได้',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'ยืนยัน',
        cancelButtonText: 'ยกเลิก'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: lockUserUrl,
                type: 'POST',
                data: { Id: id, type: 0 },
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
                        title: "Locked successfully"
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
