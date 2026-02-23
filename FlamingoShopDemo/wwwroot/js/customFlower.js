const bouquetImg = {
    rose: "https://images.unsplash.com/photo-1526047932273-341f2a7631f9",
    lily: "https://images.unsplash.com/photo-1509042239860-f550ce710b93",
    mix: "https://images.unsplash.com/photo-1501004318641-b39e6451bec6"
};

// ================= STATE =================
let state = JSON.parse(localStorage.getItem("bouquet")) || {
    flowerId: null,
    flowerPrice: 0,
    paperId: null,
    paperPrice: 0,
    stems: 12,
    cart: []
};


let uploadedRefFile = null;
let uploadedRefUrl = "";


let customBouquets = JSON.parse(localStorage.getItem("customBouquets")) || [];

function save() {
    localStorage.setItem("bouquet", JSON.stringify(state));
}

// ================= INIT =================
document.addEventListener("DOMContentLoaded", function () {

    const stemInput = document.getElementById("stemInput");
    const stemRange = document.getElementById("stemRange");

    // ---------- เลือกสินค้า (ดอก / กระดาษ / อื่น ๆ) ----------
    document.querySelectorAll(".paper-item").forEach(item => {

        item.addEventListener("click", function () {

            // ลบ active ใน card เดียวกัน
            this.closest(".card-ui")
                .querySelectorAll(".paper-item")
                .forEach(x => x.classList.remove("active"));

            this.classList.add("active");

            const price = parseFloat(this.dataset.price || 0);
            const id = this.dataset.id;

            // แยกว่าอยู่ STEP ไหน
            const stepText = this.closest(".card-ui")
                .querySelector("small").innerText;

            if (stepText.includes("ดอกไม้")) {
                state.flowerId = id;
                state.flowerPrice = price;
            }
            else if (stepText.includes("กระดาษ")) {
                state.paperId = id;
                state.paperPrice = price;
            }

            document.getElementById("aiResult").src = "";
            state.cart = [];
            save();


            checkTemplate();
            calcTotal();
        });
    });

    // ---------- จำนวนดอก ----------
    function updateStem(value) {

        value = parseInt(value);

        if (isNaN(value) || value < 1) value = 1;
        if (value > 99) value = 99;

        state.stems = value;

        stemInput.value = value;
        stemRange.value = value;

        calcTotal();
    }

    stemInput.addEventListener("input", function () {
        updateStem(this.value);
    });

    stemRange.addEventListener("input", function () {
        updateStem(this.value);
    });

    calcTotal();
});


// ================= คำนวณราคา =================
function calcTotal() {

    let total = 0;

    // ราคาดอกไม้
    if (state.flowerPrice) {
        total += state.flowerPrice;

        // เพิ่มราคาตามจำนวนดอก
        total += (state.stems - 1) * 40;
    }

    // ราคากระดาษ
    if (state.paperPrice) {
        total += state.paperPrice;
    }

    const totalEl = document.getElementById("totalPrice");
    if (totalEl) {
        totalEl.innerText = total.toLocaleString() + " บาท";
    }

    save();
}


// เลือก Reference Image
document.querySelectorAll(".ref-item").forEach(img => {

    img.addEventListener("click", function () {

        document.querySelectorAll(".ref-item")
            .forEach(x => x.classList.remove("active"));

        this.classList.add("active");

        // เปลี่ยน preview
        document.getElementById("aiResult").src = this.src;

        const id = this.dataset.id;
        const name = this.dataset.name;
        const price = parseFloat(this.dataset.price);
        const image = this.dataset.img;

        state.cart = state.cart.filter(item => item.type !== "template");

        state.cart.push({
            type: "template",   
            id: id,
            name: name,
            price: price,
            image: image,
            qty: 1
        });


        $('#totalPrice').text(price + " บาท" );
        save();
    });

});


async function generateImage() {

    document.getElementById("previewLoading")
        .classList.remove("d-none");

    fetch("/api/generate-image", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(state)
    })
        .then(res => res.blob())
        .then(blob => {

            document.getElementById("aiResult").src =
                URL.createObjectURL(blob);

            document.getElementById("previewLoading")
                .classList.add("d-none");
        });
}

function scrollRef(direction, id) {

    const container = document.getElementById(id);

    if (!container) return;   

    const scrollAmount = 250;

    container.scrollBy({
        left: direction * scrollAmount,
        behavior: "smooth"
    });
}


document.addEventListener("DOMContentLoaded", function () {

    let stemCount = 12;

    const stemInput = document.getElementById("stemInput");
    const stemRange = document.getElementById("stemRange");

    function updateStem(value) {

        if (isNaN(value) || value < 1) value = 1;
        if (value > 999) value = 999;

        stemCount = value;

        stemInput.value = stemCount;
        stemRange.value = stemCount;
    }

    stemInput.addEventListener("input", function () {
        updateStem(parseInt(this.value));
    });

    stemRange.addEventListener("input", function () {
        updateStem(parseInt(this.value));
    });

});

function checkTemplate() {
    var customBouquet = new FormData();
    customBouquet.append("flower", state.flowerId);
    customBouquet.append("qty", state.stems);
    customBouquet.append("paper", state.paperId);

    $.ajax({
        url: checkTemplateUrl,
        type: 'POST',
        data: customBouquet,
        processData: false,
        contentType: false,
        success: function (res) {
            console.log(res);
            if (!res.found) {
                document.getElementById("templateResultBox")
                    .classList.add("d-none");
                return;
            }

            const box = document.getElementById("templateResultBox");
            const container = document.getElementById("templateScroll");



            box.classList.remove("d-none");
            container.innerHTML = "";

            //res.data.forEach(item => {

            //    container.innerHTML += `
            //         <img src="${item.imagePath}"
            //         class="template-item ref-item"
            //         data-id="${item.tempFlowerId}">
            //     `;
            //});

            res.data.forEach(item => {
                container.innerHTML += `
                                    <img src="${item.imagePath}"
                                         class="template-item ref-item"
                                         data-id="${item.tempFlowerId}"
                                         data-name="${item.name}"
                                         data-img="${item.imagePath}"
                                         data-price="${item.price}">
                                `;
            });



          
           
        },
        error: function () {
            Swal.fire('Error', 'Can not Delete', 'error');
        }
    });
}


document.getElementById("templateScroll")
    .addEventListener("click", function (e) {
        document.getElementById("wrapPreview").classList.remove("d-none");

        if (!e.target.classList.contains("template-item"))
            return;

        const el = e.target;

        // active class
        document.querySelectorAll(".template-item")
            .forEach(x => x.classList.remove("active"));

        el.classList.add("active");

        // preview
        document.getElementById("aiResult").src = el.src;

        const id = el.dataset.id;
        const name = el.dataset.name;
        const price = parseFloat(el.dataset.price);
        const image = el.dataset.img;

        // ลบ template เก่า
        state.cart = state.cart.filter(item => item.type !== "template");

        // เพิ่มใหม่
        state.cart.push({
            type: "template",
            id: id,
            name: name,
            price: price,
            image: image,
            qty: 1
        });

        $('#totalPrice').text(price + " บาท");

        save();

        console.log("Updated cart:", state.cart);
    });

async  function addToCartCustom()
{

    if (state.cart && state.cart.length > 0) {

        const item = state.cart[0];

        addToCart(
            item.name,
            item.price,
            item.id,
            item.image
        );
    }
    else
    {
        let customId = Date.now();
        let imageUrl = "";

        if (uploadedRefFile) {

            const formData = new FormData();
            formData.append("Image", uploadedRefFile);
            formData.append("type", 2);
            formData.append("id", customId); 

            const response = await fetch("@Url.Action('SaveImages','Base')'", {
                method: "POST",
                body: formData
            });

            const result = await response.json();
            imageUrl = result.imageUrl; 
        }

        let priceText = $('#totalPrice').text().trim();
        let firstWord = priceText.split(' ')[0];

        let cart = JSON.parse(localStorage.getItem("cart")) || [];

        cart.push({
            id: customId,          
            name: "Customization",
            price: firstWord,
            img: imageUrl,         
            qty: 1
        });

        localStorage.setItem("cart", JSON.stringify(cart));

        let customItem = {
            customId: Date.now(),
            flowerId: state.flowerId,
            flowerPrice: state.flowerPrice,
            paperId: state.paperId,
            paperPrice: state.paperPrice,
            stems: state.stems,
            totalPrice: (state.flowerPrice * state.stems) + state.paperPrice
        };

        let customBouquets = JSON.parse(localStorage.getItem("customBouquets")) || [];
        customBouquets.push(customItem);
        localStorage.setItem("customBouquets", JSON.stringify(customBouquets));

        updateCartUI();

    }


}

function previewCustomerRef(event) {
    const file = event.target.files[0];
    if (!file) return;

    uploadedRefFile = file; 
    const reader = new FileReader();
    reader.onload = function (e) {
        document.getElementById("customerRefPreview").src = e.target.result;
        document.getElementById("customerRefPreview").classList.remove("d-none");
        document.getElementById("uploadPlaceholder").classList.add("d-none");
        document.getElementById("wrapPreview").classList.add("d-none");
    };

    reader.readAsDataURL(file);
}