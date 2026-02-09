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



document.querySelectorAll(".add-cart-btn").forEach(btn => {
    btn.addEventListener("click", function () {
        addToCart(
            this.dataset.name,
            this.dataset.price,
            this.dataset.id,
            this.dataset.img
        );
    });
});


function addToCart(name, price, id, img) {
    cart.push({
        id: id,
        img: img,
        name: name,
        price: price,
        qty: 1
    });

    localStorage.setItem("cart", JSON.stringify(cart));
    updateCartCount();

}

function updateCartCount() {
    const cart = JSON.parse(localStorage.getItem("cart")) || [];
    const cartCount = document.getElementById("cartCount");

    if (cart.length > 0) {
        cartCount.innerText = cart.length;
        cartCount.style.display = "inline-block";
    } else {
        cartCount.style.display = "none";
    }
    renderCart();
}

function renderCart() {
    const box = document.getElementById("cartItems");
    const totalEl = document.getElementById("cartTotal");

    if (!box) return;

    box.innerHTML = "";
    let total = 0;

    if (!cart || cart.length === 0) {
        box.innerHTML = "<p class='text-muted'>ยังไม่มีสินค้า</p>";
        totalEl.innerText = "0 บาท";
        return;
    }

    const groupedCart = {};

    cart.forEach(item => {
        if (groupedCart[item.id]) {
            groupedCart[item.id].qty += item.qty || 1;
        } else {
            groupedCart[item.id] = {
                ...item,
                qty: item.qty || 1
            };
        }
    });

    Object.values(groupedCart).forEach(c => {
        total += c.price * c.qty;

        box.insertAdjacentHTML("beforeend", `
            <div class="d-flex gap-3 mb-3 align-items-center">
                <img src="${c.img}" width="60" class="rounded">

                <div class="flex-grow-1">
                    <p class="mb-1 fw-medium">${c.name}</p>
                    <small class="text-muted">
                        ${c.qty} × ฿${c.price}
                    </small>
                </div>

                <i class="bi bi-x text-danger"
                   role="button"
                   onclick="removeFromCart(${c.id})"></i>
            </div>
        `);
    });

    totalEl.innerText = formatTHB ( total);
}

function removeFromCart(id) {
    cart = cart.filter(item => item.id.toString() !== id.toString());

    localStorage.setItem("cart", JSON.stringify(cart));
    //renderCart();
    updateCartCount();
}


document.addEventListener("DOMContentLoaded", function () {
    updateCartCount();


    const cartCanvas = document.getElementById("cartCanvas");
    cartCanvas.addEventListener("shown.bs.offcanvas", function () {
        updateCartCount();
    });
});


function formatTHB(amount) {
    return new Intl.NumberFormat('th-TH', {
        style: 'currency',
        currency: 'THB'
    }).format(amount);
}


const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))