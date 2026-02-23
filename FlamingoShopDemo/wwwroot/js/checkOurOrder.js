let map;
let marker;
const input = document.getElementById("deliveryDateTime");


function init() {
    var subtotal = parseInt(localStorage.getItem("subTotal")) || 0;
    var shipping = 100;
    var total = formatTHB(subtotal + shipping);
    $('#totalPayment').text(total);
    $('#paymentAmountModal').text(total);

    initMap();

}

function formatTHB(amount) {
    return new Intl.NumberFormat('th-TH', {
        style: 'currency',
        currency: 'THB'
    }).format(amount);
}

function initMap() {

    const defaultLocation = { lat: 13.7563, lng: 100.5018 }; // Bangkok

    map = new google.maps.Map(document.getElementById("map"), {
        center: defaultLocation,
        zoom: 13
    });

    marker = new google.maps.Marker({
        position: defaultLocation,
        map: map,
        draggable: true
    });

    const input = document.getElementById("addressInput");
    const autocomplete = new google.maps.places.Autocomplete(input);

    autocomplete.addListener("place_changed", function () {
        const place = autocomplete.getPlace();

        if (!place.geometry) return;

        const location = place.geometry.location;

        map.setCenter(location);
        map.setZoom(16);

        marker.setPosition(location);

        document.getElementById("lat").value = location.lat();
        document.getElementById("lng").value = location.lng();
    });

    // ลากหมุดได้
    marker.addListener("dragend", function () {
        const position = marker.getPosition();
        document.getElementById("lat").value = position.lat();
        document.getElementById("lng").value = position.lng();
    });
}

function getNowFormatted() {
    const now = new Date();
    const offset = now.getTimezoneOffset();
    const localNow = new Date(now.getTime() - (offset * 60000));
    return localNow.toISOString().slice(0, 16);
}

function setMinDateTime() {
    input.min = getNowFormatted();
}

function validateDateTime() {
    const selected = new Date(input.value);
    const now = new Date();

    if (selected < now) {
        Swal.fire({
            icon: "warning",
            title: "เลือกเวลาไม่ถูกต้อง",
            text: "ไม่สามารถเลือกเวลาย้อนหลังได้",
            confirmButtonText: "ตกลง",
            confirmButtonColor: "#c79a8b"
        });

        input.value = getNowFormatted();
    }
}

/* payment */
document.querySelectorAll('.payment').forEach(p => {
    p.onclick = () => {
        document.querySelectorAll('.payment').forEach(x => x.classList.remove('active'));
        p.classList.add('active');
    };
});
input.addEventListener("change", validateDateTime);
function onClickConfirm() {
    var orderDraft = new FormData();

    let address = $('#addressInput').val().trim();
    let tel = $('#telInput').val().trim();
    let name = $('#nameInput').val().trim();
    let date = $('#deliveryDateTime').val().trim();
    if (!address || !tel || !name || !date) {

        Swal.fire({
            icon: "warning",
            title: "ข้อมูลไม่ครบ",
            text: "กรุณากรอกข้อมูลให้ครบทุกช่อง",
            confirmButtonColor: "#c79a8b"
        });

        return; 
    }
    orderDraft.append("AddressDelivary", address);
    orderDraft.append("TelephoneOrder", tel);
    orderDraft.append("fullNameRecrive", name);
    orderDraft.append("OrderDate", date);
    var subtotal = parseInt(localStorage.getItem("subTotal")) || 0;
    var shipping = 100;
    orderDraft.append("TotalPrice", (subtotal + shipping));

    let cart = JSON.parse(localStorage.getItem("cart")) || [];
    cart.forEach((item, index) => {
        orderDraft.append(`OrderSubDraft[${index}].TemplateId`, Number(item.id));
        orderDraft.append(`OrderSubDraft[${index}].Price`, item.price);
        orderDraft.append(`OrderSubDraft[${index}].Qty`, item.qty);
    });

    let customBouquets = JSON.parse(localStorage.getItem("customBouquets")) || [];
    customBouquets.forEach((item, index) => {
        orderDraft.append(`OrderDetailDraft[${index}].CustomId`, item.customId);
        orderDraft.append(`OrderDetailDraft[${index}].FlowerId`, item.flowerId);
        orderDraft.append(`OrderDetailDraft[${index}].FlowerPrice`, item.flowerPrice);
        orderDraft.append(`OrderDetailDraft[${index}].PaperId`, item.paperId);
        orderDraft.append(`OrderDetailDraft[${index}].PaperPrice`, item.paperPrice);
        orderDraft.append(`OrderDetailDraft[${index}].Stems`, item.stems);
        orderDraft.append(`OrderDetailDraft[${index}].TotalPrice`, item.totalPrice);
    });


    $.ajax({
        url: addOrderMaster,
        type: 'POST',
        data: orderDraft,
        contentType: false,
        processData: false,
        success: function (res) {
            var cart = [];

            saveCart(cart);
            updateCartUI();

            const paymentModal = new bootstrap.Modal(
                document.getElementById('paymentModal')
            );

            paymentModal.show();
        }

    });

}

function confirmPayment() {

    const fileInput = document.getElementById("slipImage");

    if (fileInput.files.length === 0) {
        Swal.fire({
            icon: "warning",
            title: "กรุณาแนบสลิปก่อน",
            confirmButtonColor: "#c79a8b"
        });
        return;
    }

    let paymentForm = new FormData();
    paymentForm.append("OrderId", window.currentOrderId); // เก็บจากตอน save order
    paymentForm.append("SlipImage", fileInput.files[0]);

    $.ajax({
        url: UploadSlip,
        type: "POST",
        data: paymentForm,
        contentType: false,
        processData: false,
        success: function (res) {

            if (res.success) {

                Swal.fire({
                    icon: "success",
                    title: "ส่งสลิปเรียบร้อย",
                    text: "กำลังตรวจสอบการชำระเงิน"
                }).then(() => {

                    localStorage.removeItem("cart");
                    localStorage.removeItem("customBouquets");

                    window.location.href = "/Customer/OrderSuccess";
                });
            }
        }
    });
}

function previewSlip(event) {
    const reader = new FileReader();
    reader.onload = function () {
        $('#slipPreview')
            .attr('src', reader.result)
            .removeClass('d-none');
    };
    reader.readAsDataURL(event.target.files[0]);
}

setMinDateTime();
setInterval(setMinDateTime, 60000);
$(document).ready(function () {
    init();
});
