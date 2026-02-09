// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const statusMap = {
    1: 'Processed',
    2: 'Shipped',
    3: 'En Route',
    4: 'Arrived'
};


const toggleBtn = document.getElementById("menu-toggle");
const sidebar = document.getElementById("sidebar");

function isMobile() {
    return window.innerWidth < 768;
}

toggleBtn.addEventListener("click", () => {
    sidebar.hidden = !sidebar.hidden;
});

sidebar.addEventListener("mouseenter", () => {
    if (!isMobile()) {
        sidebar.classList.remove("collapsed");
    }
});

sidebar.addEventListener("mouseleave", () => {
    if (!isMobile()) {
        sidebar.classList.add("collapsed");
    }
});

window.addEventListener("resize", () => {
    if (isMobile()) {
        sidebar.hidden = true;
        toggleBtn.style.display = "block";
    } else {
        sidebar.hidden = false;
        toggleBtn.style.display = "none";
    }
});

if (isMobile()) {
    sidebar.hidden = true;
    toggleBtn.style.display = "block";
} else {
    sidebar.hidden = false;
    toggleBtn.style.display = "none";
}

document.querySelectorAll('.order-card').forEach(card => {
    const status = parseInt(card.dataset.status); // 1-4
    const steps = card.querySelectorAll('.progress-step');
    const labels = card.querySelectorAll('.step-label');

    steps.forEach((step, index) => {
        if (index < status) {
            step.classList.add('active');
        }
    });

    labels.forEach((label, index) => {
        if (index >= status) {
            label.classList.add('inactive');
        }
    });
});

document.querySelectorAll('.btn-detail').forEach(btn => {
    btn.addEventListener('click', function () {
        const card = this.closest('.order-card');
        const orderId = card.dataset.orderId;
        const detail = document.getElementById('detail-' + orderId);

        document.querySelectorAll('.order-detail').forEach(d => {
            if (d !== detail) d.classList.add('d-none');
        });

        detail.classList.toggle('d-none');
    });
});
function renderProgress(card, status) {
    const steps = card.querySelectorAll('.progress-step');
    const labels = card.querySelectorAll('.step-label');
    const dropdownBtn = card.querySelector('.btn-status');
    const dropdownItems = card.querySelectorAll('.dropdown-item');

    // progress
    steps.forEach((step, index) => {
        step.classList.remove('done', 'current');
        step.innerHTML = '';

        if (index + 1 < status) {
            step.classList.add('done');
            step.innerHTML = '<i class="bi bi-check"></i>';
        } else if (index + 1 === status) {
            step.classList.add('current');
        }
    });

    // labels
    labels.forEach((label, index) => {
        label.classList.remove('done', 'current');

        if (index + 1 < status) {
            label.classList.add('done');
        } else if (index + 1 === status) {
            label.classList.add('current');
        }
    });

    // dropdown button text
    if (dropdownBtn) {
        dropdownBtn.textContent = statusMap[status];
    }

    // dropdown active item
    dropdownItems.forEach(item => {
        item.classList.toggle(
            'active',
            parseInt(item.dataset.status) === status
        );
    });

    card.dataset.status = status;
}

// initial
document.querySelectorAll('.order-card').forEach(card => {
    renderProgress(card, parseInt(card.dataset.status));
});

// dropdown click
document.querySelectorAll('.dropdown-item').forEach(item => {
    item.addEventListener('click', function (e) {
        e.preventDefault();
        const status = parseInt(this.dataset.status);
        const card = this.closest('.order-card');
        renderProgress(card, status);
    });
});

document.addEventListener('DOMContentLoaded', () => {

    document.querySelectorAll('.material-item').forEach(card => {
        const stock = parseInt(card.dataset.stock, 10);

        // ลบ badge เดิม (กันค้าง)
        card.querySelector('.stock-badge')?.remove();
        card.classList.remove('low-stock');

        if (!isNaN(stock) && stock <= 10) {
            card.classList.add('low-stock');

            const badge = document.createElement('div');
            badge.className = 'stock-badge';
            badge.innerText = 'LOW STOCK';

            card.appendChild(badge);
        }
    });

});