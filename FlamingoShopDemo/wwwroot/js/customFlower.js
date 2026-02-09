const bouquetImg = {
    rose: "https://images.unsplash.com/photo-1526047932273-341f2a7631f9",
    lily: "https://images.unsplash.com/photo-1509042239860-f550ce710b93",
    mix: "https://images.unsplash.com/photo-1501004318641-b39e6451bec6"
};

function save() { localStorage.setItem("bouquet", JSON.stringify(state)); }


let state = JSON.parse(localStorage.getItem("bouquet")) || {
    flower: "rose", stems: 12, wrap: "#f2c1cc", extras: [], cart: []
};
function updatePreview() {
    bouquetImg[state.flower] && (bouquetImg[state.flower]);
    document.getElementById("aiResult").src = bouquetImg[state.flower];
    document.getElementById("wrapPreview").style.background = state.wrap;
    document.getElementById("stemCount").innerText = state.stems;

    generateImage();

    calcTotal();
}

function generatePrompt() {
   return   `A realistic flower bouquet photo.
            Flower type: ${state.flower}
            Number of stems: ${state.stems}
            Wrapping paper color: ${state.wrap}
            Studio lighting, product photography, white background
            `;
}

async function generateImage() {
    fetch("/api/generate-image", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(state)
    })
        .then(res => res.blob())
        .then(blob => {
            document.getElementById("aiResult").src =
                URL.createObjectURL(blob);
        });
}


function calcTotal() {
    let base = { rose: 1599, lily: 1699, mix: 1899 }[state.flower];
    let total = base + (state.stems - 12) * 40;
    state.extras.forEach(e => total += e);
    document.getElementById("totalPrice").innerText = total.toLocaleString() + " บาท";
    save();
}

document.querySelectorAll(".flower-option").forEach(f => {
    f.onclick = () => {
        document.querySelectorAll(".flower-option").forEach(x => x.classList.remove("active"));
        f.classList.add("active");
        state.flower = f.dataset.type;
        updatePreview();
    };
});

stemRange.oninput = e => {
    state.stems = e.target.value;
    updatePreview();
};

document.querySelectorAll(".color-dot").forEach(c => {
    c.onclick = () => {
        document.querySelectorAll(".color-dot").forEach(x => x.classList.remove("active"));
        c.classList.add("active");
        state.wrap = c.style.background;
        updatePreview();
    };
});

document.querySelectorAll(".extra").forEach(e => {
    e.onchange = () => {
        state.extras = [...document.querySelectorAll(".extra:checked")].map(x => +x.dataset.price);
        calcTotal();
    };
});

function addToCart() {
    state.cart.push({ ...state });
    save();
    renderCart();
}

function renderCart() {
    cartItems.innerHTML = "";
    state.cart.forEach(c => {
        cartItems.innerHTML += `
    <div class="mb-3">
      <img src="${bouquetImg[c.flower]}" class="w-100 rounded mb-1">
      <small>${c.stems} ดอก</small>
    </div>`;
    });
}

document.querySelectorAll(".recommend").forEach(card => {
    card.onclick = () => {
        state.flower = card.dataset.flower;
        state.stems = +card.dataset.stems;
        stemRange.value = state.stems;
        document.querySelectorAll(".flower-option").forEach(f => {
            f.classList.toggle("active", f.dataset.type === state.flower);
        });
        updatePreview();
        window.scrollTo({ top: 0, behavior: "smooth" });
    };
});

updatePreview();
renderCart();