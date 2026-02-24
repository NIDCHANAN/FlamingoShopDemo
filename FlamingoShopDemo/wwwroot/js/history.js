document.querySelectorAll(".toggle-detail").forEach(btn => {
    btn.addEventListener("click", function () {
        const card = this.closest(".order-history-card");
        const detail = card.querySelector(".order-detail");
        detail.classList.toggle("d-none");
    });
});


let currentOrderId = null;

function setPaymentData(orderId, amount) {
    currentOrderId = orderId;
    document.getElementById("paymentAmountModal").innerText = "฿ " + amount;
}

function confirmPayment() {

    if (!currentOrderId) {
        alert("ไม่พบคำสั่งซื้อ");
        return;
    }

    const fileInput = document.getElementById("slipImage");

    if (!fileInput.files.length) {
            Swal.fire({
                icon: "warning",
                title: "กรุณาแนบสลิปก่อน",
                confirmButtonColor: "#c79a8b"
            });
            return;
    }

    let paymentForm = new FormData();
    paymentForm.append("OrderId", currentOrderId);
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

                    window.location.href = "/Customer/History";
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