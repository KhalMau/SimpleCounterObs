// ===== Config =====
// Nombre del archivo que tu app escribe. Cambia aquí si usas otro.
// Se recomienda "state.json". El script intentará "overlay-data.json" como fallback.
const STATE_FILES = ["state.json", "overlay-data.json"];

// Frecuencia de actualización (ms). 500–1000 suele ir bien.
const POLL_MS = 500;

// ===== Lógica =====
const $root  = document.getElementById("root");
const $title = document.getElementById("title");
const $value = document.getElementById("value");

// Valores por defecto (si el JSON no trae algo)
const defaults = {
  title: "Contador",
  value: 0,
  style: {
    fontFamily: "Inter, Arial, system-ui, Segoe UI, sans-serif",
    fontSizePx: 45,
    minimal: false,
    showTitle: true
  }
};

// Último snapshot para evitar re-render si no cambió
let lastSnapshot = "";

function stateToSnapshot(obj) {
  // serialización básica y estable para comparar cambios
  try { return JSON.stringify(obj); } catch { return ""; }
}

async function fetchFirstAvailable(files) {
  for (const name of files) {
    try {
      const url = `${name}?_=${Date.now()}`; // bust de caché
      const res = await fetch(url, { cache: "no-store" });
      if (res.ok) return await res.json();
    } catch {
      // ignora y prueba el siguiente
    }
  }
  throw new Error("No se pudo cargar el estado local.");
}

function applyState(state) {
  const s = {
    title: (state?.Title ?? state?.title ?? defaults.title),
    value: (state?.Value ?? state?.value ?? defaults.value),
    style: {
      fontFamily: (state?.Style?.FontFamily ?? state?.style?.fontFamily ?? defaults.style.fontFamily),
      fontSizePx: (state?.Style?.FontSizePx ?? state?.style?.fontSizePx ?? defaults.style.fontSizePx),
      minimal:    (state?.Style?.Minimal    ?? state?.style?.minimal    ?? defaults.style.minimal),
      showTitle:  (state?.Style?.ShowTitle  ?? state?.style?.showTitle  ?? defaults.style.showTitle),
    }
  };

  // Actualiza solo si cambiaron los datos
  const snap = stateToSnapshot(s);
  if (snap === lastSnapshot) return;
  lastSnapshot = snap;

  // Título
  $title.textContent = s.title ?? defaults.title;
  $title.style.display = s.style.showTitle ? "block" : "none";

  // Valor
  $value.textContent = String(s.value ?? 0);

  // Estilos
  document.documentElement.style.setProperty("--font-family", s.style.fontFamily);
  document.documentElement.style.setProperty("--font-size", `${s.style.fontSizePx}px`);

  // Minimal
  if (s.style.minimal) $root.classList.add("minimal");
  else $root.classList.remove("minimal");
}

async function tick() {
  try {
    const state = await fetchFirstAvailable(STATE_FILES);
    applyState(state);
  } catch (err) {
    // Si no encuentra archivo, no spamear errores; solo deja el último render
    // console.warn(err);
  }
}

// Arranque
tick();
setInterval(tick, POLL_MS);

