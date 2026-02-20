const scrollTopBtn = document.getElementById("scrollTopBtn");
let cart = JSON.parse(localStorage.getItem("cart")) || [];

window.onscroll = function () {
    if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
        scrollTopBtn.style.display = "block";
    } else {
        scrollTopBtn.style.display = "none";
    }
};

scrollTopBtn.addEventListener("click", function () {
    window.location.href = "https://lin.ee/en2DKTa";
});



//document.querySelectorAll(".add-cart-btn").forEach(btn => {
//    btn.addEventListener("click", function () {
//        addToCart(
//            this.dataset.name,
//            this.dataset.price,
//            this.dataset.id,
//            this.dataset.img
//        );
//    });
//});


document.addEventListener("click", function (e) {

    const btn = e.target.closest(".add-cart-btn");
    if (!btn) return;


    const name = btn.dataset.name;
    const img = btn.dataset.img;
    const price = parseFloat(btn.dataset.price);
    const id = btn.dataset.id;

    addToCart(name, price, id, img);

    // เอฟเฟกต์เล็กๆ เวลากด
    btn.innerText = "✓ เพิ่มแล้ว";
    btn.classList.add("btn-success");

    setTimeout(() => {
        btn.innerText = "เพิ่มลงตะกร้า";
        btn.classList.remove("btn-success");
    }, 1000);

});

// ============================
// CART CORE
// ============================

function getCart() {
    return JSON.parse(localStorage.getItem("cart")) || [];
}

function saveCart(cart) {
    localStorage.setItem("cart", JSON.stringify(cart));
}

function addToCart(name, price, id, img) {

    if (!id) {
        console.error("Product id is undefined");
        return;
    }

    let cart = JSON.parse(localStorage.getItem("cart")) || [];

    const existing = cart.find(item => String(item.id) === String(id));

    
    if (existing) {
        existing.qty += 1;
    } else {
        cart.push({
            id: id,
            name: name,
            price: price,
            img: img || "/images/no-image.png",
            qty: 1
        });
    }

    localStorage.setItem("cart", JSON.stringify(cart));
    updateCartUI();
}



// ============================
// UPDATE COUNT + RENDER
// ============================

function updateCartUI() {

    const cart = getCart();
    const totalQty = cart.reduce((sum, item) => sum + (item.qty || 1), 0);

    // UPDATE COUNT (รองรับหลายจุด)
    document.querySelectorAll(".cartCount").forEach(el => {
        if (totalQty > 0) {
            el.innerText = totalQty;
            el.style.display = "flex";
        } else {
            el.style.display = "none";
        }
    });

    renderCart();
}


// ============================
// RENDER CART
// ============================

function renderCart() {

    const cart = getCart();
    const containers = document.querySelectorAll(".cartItems");
    const totalEls = document.querySelectorAll(".cartTotal");

    const containersCheckout = document.querySelectorAll(".orderList");
    const totalCheckout = document.querySelectorAll(".paymentAmount");


    if (containers.length === 0 ) return;
    let total = 0;

    if (cart.length === 0) {
        containers.forEach(box => {
            box.innerHTML = "<p class='text-muted'>ยังไม่มีสินค้า</p>";
        });

        containersCheckout.forEach(box => {
            box.innerHTML = "<p class='text-muted'>ยังไม่มีสินค้า</p>";
        });


        totalEls.forEach(el => el.innerText = "฿0");
        return;
    }

    containers.forEach(box => box.innerHTML = "");
    containersCheckout.forEach(box => box.innerHTML = "");

    cart.forEach(c => {

        total += c.price * (c.qty || 1);

        const itemHTML = `
                            <div class="d-flex align-items-center gap-2 mb-3 border-bottom pb-2">

                                <img src="${c.img}" width="60" height="60"
                                     class="rounded flex-shrink-0 object-fit-cover">

                                <div class="flex-grow-1 min-w-0">
                                    <p class="mb-1 fw-medium text-truncate">${c.name}</p>
                                    <small class="text-muted d-block text-truncate">
                                        ${c.qty} × ฿${c.price}
                                    </small>
                                </div>

                                <button class="btn btn-sm btn-light text-danger border-0 flex-shrink-0"
                                        onclick="removeFromCart('${c.id}')">
                                    <i class="bi bi-x-lg"></i>
                                </button>

                            </div>
                        `;


        containers.forEach(box => {
            box.insertAdjacentHTML("beforeend", itemHTML);
        });

        containersCheckout.forEach(box => {
            box.insertAdjacentHTML("beforeend", itemHTML);
        });
    });

    totalEls.forEach(el => {
        el.innerText = formatTHB(total).toLocaleString();
    });


    totalCheckout.forEach(el => {
        el.innerText = formatTHB(total).toLocaleString();
        localStorage.setItem("subTotal", total);

    });
}


// ============================
// REMOVE
// ============================

function removeFromCart(id) {


    let cart = getCart();

    cart = cart.filter(item => item.id !== id);

    saveCart(cart);
    updateCartUI();
}


// ============================
// INIT
// ============================

document.addEventListener("DOMContentLoaded", function () {
    updateCartUI();
});



function formatTHB(amount) {
    return new Intl.NumberFormat('th-TH', {
        style: 'currency',
        currency: 'THB'
    }).format(amount);
}


const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))