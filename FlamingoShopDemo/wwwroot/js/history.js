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

function setReviewData(orderId) {
    document.getElementById("reviewOrderId").value = orderId;
    document.getElementById("reviewScore").value = 0;
    document.getElementById("reviewComment").value = "";

    document.querySelectorAll(".star").forEach(s => {
        s.classList.remove("active");
        s.classList.remove("bi-star-fill");
        s.classList.add("bi-star");
    });
}

document.querySelectorAll(".star").forEach(star => {
    star.addEventListener("click", function () {
        let value = this.getAttribute("data-value");
        document.getElementById("reviewScore").value = value;

        document.querySelectorAll(".star").forEach(s => {
            s.classList.remove("active");
            s.classList.remove("bi-star-fill");
            s.classList.add("bi-star");
        });

        for (let i = 0; i < value; i++) {
            let starEl = document.querySelectorAll(".star")[i];
            starEl.classList.add("active");
            starEl.classList.remove("bi-star");
            starEl.classList.add("bi-star-fill");
        }
    });
});


function submitReview() {
    let orderId = document.getElementById("reviewOrderId").value;
    let score = document.getElementById("reviewScore").value;
    let comment = document.getElementById("reviewComment").value;

    if (score == 0) {
        Swal.fire({
            icon: 'warning',
            title: 'ยังไม่ได้ให้คะแนน',
            text: 'กรุณาให้คะแนนก่อนนะคะ 💕',
            confirmButtonColor: '#ff4da6'
        });
        return;
    }

    $.ajax({
        url: Review,
        type: 'POST',
        data: {
            OrderId: orderId,
            rang: score,
            comment: comment
        },
        success: function (res) {
            if (res.found) {
                Swal.fire({
                    icon: 'success',
                    title: 'รีวิวสำเร็จ 🌸',
                    text: 'ขอบคุณสำหรับความคิดเห็นของคุณ'
                }).then(() => {
                    $('#reviewModal').modal('hide');
                    location.reload();
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'ไม่พบรายการ',
                    text: 'กรุณาลองใหม่อีกครั้ง',
                    confirmButtonColor: '#ff4da6'
                });
            }

        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'เกิดข้อผิดพลาด',
                text: 'ไม่สามารถบันทึกรีวิวได้',
                confirmButtonColor: '#ff4da6'
            });
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