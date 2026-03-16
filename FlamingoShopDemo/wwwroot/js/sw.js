const CACHE_NAME = "flamingo-cache-v1";

const urlsToCache = [
    "/",
    "/css/site.css",
    "/css/login.css",
    "/css/customer.css",

    "/js/site.js",
    "/js/admin.js",
    "/js/login.js",
    "/js/user.js",
    "/js/order.js",
    "/js/product.js",

    "/favicon.ico"
];

self.addEventListener("install", event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                return cache.addAll(urlsToCache);
            })
    );
});

self.addEventListener("fetch", event => {
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                return response || fetch(event.request);
            })
    );
});