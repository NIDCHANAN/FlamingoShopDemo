

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
