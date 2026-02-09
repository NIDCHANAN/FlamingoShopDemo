/* calendar */
const calendar = document.getElementById('calendar');
const days = ['S', 'M', 'T', 'W', 'T', 'F', 'S'];

days.forEach(d => {
    const h = document.createElement('div');
    h.textContent = d;
    h.className = 'head';
    calendar.appendChild(h);
});

const startOffset = 3; // จัดตำแหน่งให้เหมือน mockup
for (let i = 0; i < startOffset; i++) {
    const e = document.createElement('div');
    e.className = 'inactive';
    calendar.appendChild(e);
}

for (let i = 1; i <= 31; i++) {
    const d = document.createElement('div');
    d.textContent = i;
    d.onclick = () => {
        document.querySelectorAll('.calendar div').forEach(x => x.classList.remove('active'));
        d.classList.add('active');
    };
    if (i === 20) d.classList.add('active');
    calendar.appendChild(d);
}

/* time */
document.querySelectorAll('.time-slot').forEach(t => {
    t.onclick = () => {
        document.querySelectorAll('.time-slot').forEach(x => x.classList.remove('active'));
        t.classList.add('active');
    };
});

/* payment */
document.querySelectorAll('.payment').forEach(p => {
    p.onclick = () => {
        document.querySelectorAll('.payment').forEach(x => x.classList.remove('active'));
        p.classList.add('active');
    };
});