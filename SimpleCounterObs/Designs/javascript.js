// Permite ajustar el intervalo desde OBS: overlay.html?interval=300
const urlParams = new URLSearchParams(location.search);
const INTERVAL_MS = Math.max(50, parseInt(urlParams.get('interval') || '300', 10));

let last = {
  version: -1,
  title: null,
  value: null,
  fontFamily: null,
  fontSize: null,
  minimal: null,
  showTitle: null
};

const titleEl = document.getElementById('title');
const valueEl = document.getElementById('value');

function applyFromState() {
  const S = (window.__STATE__) || {};
  const Style = S.Style || {};

  const title     = S.Title ?? 'Contador';
  const value     = S.Value ?? 0;
  const version   = S.Version ?? 0;
  const font      = Style.FontFamily || 'Segoe UI';
  const sizePx    = (Style.FontSize ?? 64) + 'px';
  const minimal   = !!Style.Minimal;
  const showTitle = (Style.ShowTitle !== false); // default true

  const changed =
    version !== last.version ||
    title   !== last.title   ||
    value   !== last.value   ||
    font    !== last.fontFamily ||
    sizePx  !== last.fontSize ||
    minimal !== last.minimal ||
    showTitle !== last.showTitle;

  if (!changed) return;

  // Texto
  if (title !== last.title) titleEl.textContent = title;
  if (value !== last.value) valueEl.textContent = value;

  // Estilo (CSS variables y clases)
  if (font !== last.fontFamily) {
    document.documentElement.style.setProperty('--font-family', `${font}, system-ui, sans-serif`);
  }
  if (sizePx !== last.fontSize) {
    document.documentElement.style.setProperty('--font-size', sizePx);
  }

  if (minimal !== last.minimal) {
    document.body.classList.toggle('minimal', minimal);
  }

  if (showTitle !== last.showTitle) {
    document.body.classList.toggle('hide-title', !showTitle);
  }

  // Guarda último estado aplicado
  last.version = version;
  last.title = title;
  last.value = value;
  last.fontFamily = font;
  last.fontSize = sizePx;
  last.minimal = minimal;
  last.showTitle = showTitle;
}

// Recarga periódica del archivo JS local para actualizar __STATE__
function refreshDataScript() {
  const old = document.getElementById('data');
  if (old) old.remove();

  const sc = document.createElement('script');
  sc.id = 'data';
  sc.src = '../overlay-data.js?cb=' + Date.now();
  sc.onload = applyFromState;
  sc.onerror = () => {/* si falta el archivo, reintenta en el próximo tick */};
  document.body.appendChild(sc);
}

// Primer intento de aplicar (por si ya estaba cargado)
applyFromState();

// Polling local
setInterval(refreshDataScript, INTERVAL_MS);
