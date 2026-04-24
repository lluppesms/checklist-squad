// Minimal service worker for Blazor Server PWA support.
// Blazor Server requires a live SignalR connection, so true offline mode
// is not possible. This service worker enables the PWA install prompt
// and caches static assets for faster startup.

const CACHE_NAME = 'rigroll-cache-v1';

// Static assets to pre-cache for faster load times
const PRECACHE_URLS = [
  '/',
  'manifest.webmanifest',
  'favicon.png',
  'images/icons/icon-192x192.png',
  'images/icons/icon-512x512.png'
];

// Install: pre-cache static assets
self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(cache => cache.addAll(PRECACHE_URLS))
      .then(() => self.skipWaiting())
  );
});

// Activate: clean up old caches
self.addEventListener('activate', event => {
  event.waitUntil(
    caches.keys().then(keys =>
      Promise.all(keys
        .filter(key => key !== CACHE_NAME)
        .map(key => caches.delete(key))
      )
    ).then(() => self.clients.claim())
  );
});

// Fetch: network-first strategy (Blazor Server needs live connection)
// Only serve from cache if the network is unavailable
self.addEventListener('fetch', event => {
  // Skip non-GET requests and SignalR/WebSocket connections
  if (event.request.method !== 'GET') return;
  const url = new URL(event.request.url);
  if (url.pathname.startsWith('/hubs/') || url.pathname.startsWith('/_blazor')) return;

  event.respondWith(
    fetch(event.request)
      .then(response => {
        // Cache successful responses for static assets
        if (response.ok && (
          url.pathname.endsWith('.css') ||
          url.pathname.endsWith('.js') ||
          url.pathname.endsWith('.png') ||
          url.pathname.endsWith('.woff2')
        )) {
          const clone = response.clone();
          caches.open(CACHE_NAME).then(cache => cache.put(event.request, clone));
        }
        return response;
      })
      .catch(() => caches.match(event.request))
  );
});
