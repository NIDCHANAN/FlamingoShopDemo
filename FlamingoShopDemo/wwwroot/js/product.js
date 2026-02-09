let selectedFiles = [];
let deletedImages = [];
let groupIds = null;


const input = document.getElementById("imageUpload");
const preview = document.getElementById("imagePreview");


const input_list = document.getElementById("bouquetImage");
const preview_list = document.getElementById("imagePreviewBouquet");


function init() {
    //$('#TemplateList').addClass('d-none');
}

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

input_list.addEventListener("change", () => {
    Array.from(input_list.files).forEach(file => {
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
        preview_list.appendChild(wrapper);
    });

    // reset input เพื่อเลือกไฟล์เดิมซ้ำได้
    input_list.value = "";
});

document.addEventListener("change", function (e) {

    if (!e.target.classList.contains("edit-input-list")) return;

    const input = e.target;
    const modal = input.closest(".modal");
    const preview = modal.querySelector(".edit-preview-list");

    if (!modal.selectedFiles) {
        modal.selectedFiles = [];
        selectedFiles = [];
    }

    Array.from(input.files).forEach(file => {
        if (!file.type.startsWith("image/")) return;

        modal.selectedFiles.push(file);
        selectedFiles.push(file);

        const index = modal.selectedFiles.length - 1;

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
            modal.selectedFiles.splice(index, 1);
            selectedFiles.splice(index, 1);
            wrapper.remove();
        };

        wrapper.appendChild(img);
        wrapper.appendChild(btn);
        preview.appendChild(wrapper);
    });

    input.value = "";
});


function toggleCategoryStatus(id, isChecked) {
    $.ajax({
        url: updateStatusGroupUrl,
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
            }, 3000);
        },
        error: function () {
            alert('Update failed');
        }
    });
}

function toggleProductStatus(id, isChecked) {
    $.ajax({
        url: updateStatusProductUrl,
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
            }, 3000);
        },
        error: function () {
            alert('Update failed');
        }
    });
}

function toggleTemplateStatus(id, isChecked) {
    $.ajax({
        url: updateStatusTempUrl,
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

function showModalItem(groupId) {
    input.value = "";
    preview.innerHTML = "";
    selectedFiles = [];

    groupIds = groupId;

    const modalItems = document.getElementById('itemModal');
    const Items = bootstrap.Modal.getInstance(modalItems);
    Items.show();
}

function onClickSaveGroup() {
    var GroupName = $('#txtGroupName').val();

    if (GroupName != null) {
        $.ajax({
            type: 'POST',
            url: addGroupUrl,
            data: { groupName: GroupName },
            success: function (result) {
                if (result.result === "success") {
                    const modalEl = document.getElementById('groupModal');
                    const modal = bootstrap.Modal.getInstance(modalEl);

                    modal.hide();

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
                        title: "Add Group successfully"
                    });

                    window.location.reload();
                } else {
                    console.log("Error:", result);
                }
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    }

}

function onClickSaveProduct() {
    var product = new FormData();
    product.append("NameCategory", $('#txtProductName').val());
    product.append("Qty", $('#txtStock').val());
    product.append("Price", $('#txtPrice').val());
    product.append("CategoryGroupId", groupIds);
    selectedFiles.forEach(file => {
        product.append("Images", file); 
    });

    $.ajax({
        url: addProductUrl,
        type: 'POST',
        data: product,
        contentType: false, 
        processData: false, 
        success: function (res) {
            const modalItems = document.getElementById('itemModal');
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
                title: "Add Group successfully"
            });

            setTimeout(() => {
                window.location.reload();
            }, 3000);
        }
    });

}

function deletedProduct(id) {
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
                url: deletedProductUrl,
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

function previewImages(productId) {
    const input = document.getElementById(`imageUpload_${productId}`);
    const preview = document.getElementById(`imagePreview_${productId}`);

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

function onClickUpdateProduct(id) {
    var product = new FormData();
    product.append("NameCategory", $('#txtProductName_' + id).val());
    product.append("Qty", $('#txtStock_' + id).val());
    product.append("Id", id);
    selectedFiles.forEach(file => {
        product.append("Images", file);
    });


    $.ajax({
        url: updateProductUrl,
        type: 'POST',
        data: product,
        contentType: false,
        processData: false,
        success: function (res) {
            const modalItems = document.getElementById('editItemModal_' + id);
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
                title: "Update Group successfully"
            });

            setTimeout(() => {
                window.location.reload();
            }, 3000);
        }
    });

}

function onClickAdjustProduct(id) {
    var product = new FormData();
    product.append("Qty", $('#txtAdjStock_' + id).val());
    product.append("Price", $('#txtAdjPrice_' + id).val());
    product.append("Id", id);

    $.ajax({
        url: adjustProductUrl,
        type: 'POST',
        data: product,
        contentType: false,
        processData: false,
        success: function (res) {
            const modalItems = document.getElementById('adjustModal_' + id);
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
                title: "Update Stock successfully"
            });

            setTimeout(() => {
                window.location.reload();
            }, 3000);
        }
    });
}

function onClickShowTemplate(data) {
    if (data == 0) {
        $('#TemplateList').removeClass('d-none');
        $('#ProductList').addClass('d-none');
    }
    else {
        $('#ProductList').removeClass('d-none');
        $('#TemplateList').addClass('d-none');
    }
   
}

function addRow(groupId) {
    const firstRow = $(`#group_${groupId} tr:first`).html();
    $(`#group_${groupId}`).append(`<tr>${firstRow}</tr>`);
}

function removeRow(btn) {
    $(btn).closest('tr').remove();
}

function addRowEdit(btn, groupId) {
    const $modal = $(btn).closest(".modal");

    const $tbody = $modal.find(`#editGroup_${groupId}`);

    const firstRowHtml = $tbody.find("tr:first").html();
    if (!firstRowHtml) return;

    $tbody.append(`<tr>${firstRowHtml}</tr>`);
}

function removeRowEdit(btn) {
    $(btn).closest('tr').remove();
}

function saveBouquet() {

    let formData = new FormData();

    formData.append("Name", $('#bouquetName').val());
    formData.append("Price", $('#bouquetPrice').val());
    selectedFiles.forEach(file => {
        formData.append("Images", file);
    });

    let details = [];

    $('tbody[id^="group_"] tr').each(function () {

        const itemId = $(this).find('.item').val();
        if (!itemId) return;

        details.push({
            CategoryDetailId: Number($(this).find('.item').val()),
            Qty: Number($(this).find('.qty').val())
        });
    });

    formData.append("Details", JSON.stringify(details));

    $.ajax({
        url: addTemplateUrl,
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            const modalItems = document.getElementById('templateModal');
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
                title: "Add Template successfully"
            });

            setTimeout(() => {
                window.location.reload();
                onClickShowTemplate(0);
            }, 3000);
        }
    });
}

function deletedTemplate(id) {
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
                url: deletedTemplateUrl,
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
                        onClickShowTemplate(0);
                    }, 3000);
                },
                error: function () {
                    Swal.fire('Error', 'Can not Delete', 'error');
                }
            });
        }
    });
}

function onClickUpdateBouquet(btn, id) {

    let formData = new FormData();

    formData.append("Id", id);
    formData.append("Name", $('#upDateBouquetName_' + id).val());
    formData.append("Price", $('#upDateBouquetPrice_' + id).val());
    selectedFiles.forEach(file => {
        formData.append("Images", file);
    });

    const $modal = $(btn).closest('.modal');

    const details = [];

    $modal.find('tbody[id^="editGroup_"] tr').each(function () {

        const itemId = $(this).find('.item').val();
        if (!itemId) return;

        details.push({
            CategoryDetailId: Number(itemId),
            Qty: Number($(this).find('.qty').val())
        });
    });

    formData.append("Details", JSON.stringify(details));

    $.ajax({
        url: updateTempUrl,
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            const modalItems = document.getElementById('editTemplateModal_' + id);
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
                title: "Update Template successfully"
            });

            setTimeout(() => {
                window.location.reload();
                onClickShowTemplate(0);
            }, 3000);
        }
    });

}


init();